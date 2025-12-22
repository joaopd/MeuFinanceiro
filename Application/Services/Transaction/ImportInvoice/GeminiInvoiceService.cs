using System.Net.Http.Json;
using System.Text.Json;
using Domain.Abstractions.ErrorHandling;
using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.ImportInvoice;

public class GeminiInvoiceService : IImportInvoiceService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiInvoiceService> _logger;

    public GeminiInvoiceService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IConfiguration config,
        ILogger<GeminiInvoiceService> logger)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
        
        _httpClient = new HttpClient();
        var apiKey = config["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini ApiKey missing no appsettings.json");
        
        _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
    }

    public async Task<Result<int>> ExecuteAsync(Guid userId, Guid cardId, IFormFile file)
    {
        try
        {
            _logger.LogInformation("ImportInvoice started - UserId: {UserId}, CardId: {CardId}, FileName: {FileName}", 
                userId, cardId, file.FileName);

            // 1. Obter categorias para contexto da IA
            var categories = await _categoryRepository.GetAllAsync();
            var categoryList = string.Join(", ", categories.Select(c => $"{c.Name} (ID:{c.Id})"));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64Pdf = Convert.ToBase64String(ms.ToArray());

            // 3. Prompt Otimizado (Focado apenas em categorias para reduzir custo de tokens)
            var prompt = $@"
                Aja como um especialista financeiro analisando faturas de cartão de crédito em PDF.
                Categorias existentes no sistema:
                [{{categoryList}}]

                Objetivo:
                Extrair corretamente as transações financeiras, ajustando especialmente a data de transações parceladas.

                    Instruções:

                1. Extraia cada transação com:
                - TransactionDate
                    - Description
                    - Amount

                2. Ignore pagamentos de fatura, estornos, créditos ou ajustes.

                3. Classificação de categorias:
                - Se a descrição for similar a uma categoria existente, use o CategoryId correspondente.
                - Caso contrário, identifique o tipo de gasto e retorne em NewCategoryName.

                4. Transações parceladas:
                - Identifique parcelamentos no formato: 'Parcela X de Y'.

                5. Regra de data (MUITO IMPORTANTE):

                a) Se a transação NÃO for parcelada:
                - Use a data informada no PDF como TransactionDate.

                    b) Se a transação FOR parcelada:
                - A data exibida no PDF pode representar apenas a data da compra original.
                - O TransactionDate DEVE representar a data da parcela correspondente ao mês da fatura atual.

                    Regra de cálculo:
                TransactionDate =
                    Data da compra original
                    + (Número da parcela atual) meses

                6. Sempre retorne a data ajustada para que a transação pertença corretamente ao mês vigente da fatura.

                    Formato de saída:
                Retorne apenas JSON válido, sem explicações adicionais.

                    Retorne APENAS um array JSON puro (sem markdown):
                [{{ ""Description"": ""string"", ""Amount"": number, ""TransactionDate"": ""YYYY-MM-DD"", ""CategoryId"": ""uuid?"", ""NewCategoryName"": ""string?"" }}]";
            
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
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API Error: {StatusCode} - {Error}", response.StatusCode, errorBody);
                return Result.Fail(FinanceErrorMessage.InternalServerError);
            }

            var result = await response.Content.ReadAsStringAsync();
            var aiItems = ParseGeminiResponse(result);

            var finalTransactions = new List<Domain.Entities.Transaction>();

            foreach (var item in aiItems)
            {
                bool isDuplicate = await _transactionRepository.ExistsDuplicateAsync(
                    userId, item.Amount, item.TransactionDate, item.Description);

                if (isDuplicate)
                {
                    _logger.LogInformation("Duplicate transaction ignored: {Desc}", item.Description);
                    continue;
                }
                
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
                        _logger.LogInformation("Creating new category suggested by AI: {Name}", suggestedName);
                        var newCat = new Domain.Entities.Category(suggestedName);
                        await _categoryRepository.InsertAsync(newCat);
                        finalCategoryId = newCat.Id;
                    }
                }

                // Criação da transação usando o cardId fornecido pelo front-end
                finalTransactions.Add(new Domain.Entities.Transaction(
                    Guid.NewGuid(), 
                    userId, 
                    finalCategoryId, 
                    item.Amount, 
                    item.TransactionDate,
                    TransactionType.EXPENSE, 
                    cardId,
                    PaymentMethod.CREDIT, 
                    1, 1, false, false, null, 
                    item.Description
                ));
            }

            if (finalTransactions.Count == 0)
            {
                _logger.LogInformation("No new transactions to import.");
                return Result.Ok(0);
            }

            // Inserção em massa para performance
            var success = await _transactionRepository.InsertBulkAsync(finalTransactions);
            
            _logger.LogInformation("ImportInvoice finished - Imported: {Count}", finalTransactions.Count);
            return success ? Result.Ok(finalTransactions.Count) : Result.Fail(FinanceErrorMessage.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during invoice import for UserId: {UserId}", userId);
            return Result.Fail(FinanceErrorMessage.UnexpectedError);
        }
    }

    private List<AIInvoiceItemDto> ParseGeminiResponse(string response)
    {
        try 
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Gemini JSON response");
            return new List<AIInvoiceItemDto>();
        }
    }
}