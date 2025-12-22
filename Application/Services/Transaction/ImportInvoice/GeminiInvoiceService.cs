using System.Net.Http.Json;
using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Transaction.ImportInvoice;

public class GeminiInvoiceService : IImportInvoiceService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICardRepository _cardRepository;
    private readonly HttpClient _httpClient;

    public GeminiInvoiceService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        ICardRepository cardRepository,
        IConfiguration config)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _cardRepository = cardRepository;
        
        _httpClient = new HttpClient();
        var apiKey = config["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini ApiKey missing no appsettings.json");
        
        // Atualização conforme a nova doc: chave de API via Header
        _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
    }

    public async Task<Result<int>> ExecuteAsync(Guid userId, Guid cardId, IFormFile file)
    {
        try
        {
            var apiKey = _httpClient.DefaultRequestHeaders.GetValues("x-goog-api-key").FirstOrDefault();
            var categories = await _categoryRepository.GetAllAsync();
            var categoryList = string.Join(", ", categories.Select(c => $"{c.Name} (ID:{c.Id})"));
            
            var cardList = await _cardRepository.GetByUserIdAsync(userId);
            var cardListStr = string.Join(", ", cardList.Select(c => $"{c.Name} (ID:{c.Id})"));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64Pdf = Convert.ToBase64String(ms.ToArray());

            var prompt = $@"Aja como um especialista financeiro. Analise esta fatura de cartão de crédito.
                Categorias existentes no sistema: [{categoryList}].
                Cartões existentes no sistema: [{cardListStr}].

                Instruções:
                1. Extraia cada transação com Data, Descrição e Valor.
                2. Se a descrição for similar a uma categoria existente, use o ID em 'CategoryId'.
                3. Se não houver similaridade, identifique o tipo de gasto (ex: Juros, Assinaturas, Alimentação) e retorne em 'NewCategoryName'.
                4. Ignore pagamentos de faturas ou créditos.
                5. Verifique pela fatura qual cartão foi usado e retorne o 'CardId' correspondente.
                
                Retorne APENAS um array JSON puro (sem markdown):
                [{{ ""Description"": ""string"", ""Amount"": number, ""TransactionDate"": ""YYYY-MM-DD"", ""CategoryId"": ""uuid?"",""CardId"": ""uuid?"", ""NewCategoryName"": ""string?"" }}]";

          
            // URL agora sem a API Key exposta na query string
            var requestBody = new
            {
                contents = new[] {
                    new { 
                        role = "user",
                        parts = new object[] { 
                            new { text = prompt },
                            new { inline_data = new { mime_type = "application/pdf", data = base64Pdf } }
                        } 
                    }
                },
                generationConfig = new {
                }
            };

            // 4. Chamada utilizando o modelo gemini-3-flash-preview
            var response = await _httpClient.PostAsJsonAsync(
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent", 
                requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result.Fail($"Erro na IA: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var aiItems = ParseGeminiResponse(result);

            var finalTransactions = new List<Domain.Entities.Transaction>();

            foreach (var item in aiItems)
            {
                Guid finalCardId = cardId; 
                if (!string.IsNullOrEmpty(item.CardId) && Guid.TryParse(item.CardId, out var idCard))
                    finalCardId = idCard;
                
                Guid finalCategoryId;
                if (!string.IsNullOrEmpty(item.CategoryId) && Guid.TryParse(item.CategoryId, out var idCat))
                {
                    finalCategoryId = idCat;
                }
                else
                {
                    var suggestedName = item.NewCategoryName ?? "Outros";
                    var existingCategory = await _categoryRepository.GetByNameAsync(suggestedName);

                    if (existingCategory != null)
                    {
                        finalCategoryId = existingCategory.Id;
                    }
                    else
                    {
                        var newCat = new Domain.Entities.Category(suggestedName);
                        await _categoryRepository.InsertAsync(newCat);
                        finalCategoryId = newCat.Id;
                    }
                }

                var transaction = new Domain.Entities.Transaction(
                    Guid.NewGuid(),
                    userId,
                    finalCategoryId,
                    item.Amount,
                    item.TransactionDate,
                    TransactionType.EXPENSE,
                    finalCardId,                 
                    PaymentMethod.CREDIT,   
                    1, 1, false, false, null,
                    item.Description        
                );

                finalTransactions.Add(transaction);
            }

            var success = await _transactionRepository.InsertBulkAsync(finalTransactions);
            return success ? Result.Ok(finalTransactions.Count) : Result.Fail("Erro ao salvar no banco.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Falha na importação: {ex.Message}");
        }
    }

    private List<AIInvoiceItemDto> ParseGeminiResponse(string response)
    {
        using var doc = JsonDocument.Parse(response);
        var text = doc.RootElement.GetProperty("candidates")[0]
                                  .GetProperty("content")
                                  .GetProperty("parts")[0]
                                  .GetProperty("text").GetString();

        var cleanJson = text?.Replace("```json", "").Replace("```", "").Trim();
        return JsonSerializer.Deserialize<List<AIInvoiceItemDto>>(cleanJson!, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        }) ?? new();
    }
}