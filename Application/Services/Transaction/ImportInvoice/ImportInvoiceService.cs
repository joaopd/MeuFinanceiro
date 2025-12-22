using System.Net.Http.Json;
using System.Text.Json;
using Domain.Abstractions.ErrorHandling;
using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.ImportInvoice;

public class ImportInvoiceService(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IInvoiceRepository invoiceRepository,
    ICardRepository cardRepository,
    IConfiguration configuration,
    ILogger<ImportInvoiceService> logger)
    : IImportInvoiceService
{
    private readonly HttpClient _httpClient = CreateHttpClient(configuration);

    public async Task<Result<int>> ExecuteAsync(Guid userId, Guid cardId, IFormFile file)
    {
        try
        {
            logger.LogInformation(
                "ImportInvoice started - UserId: {UserId}, CardId: {CardId}, FileName: {FileName}",
                userId, cardId, file.FileName);

            var card = await cardRepository.GetByIdAsync(cardId);
            if (card is null)
                return Result.Fail(FinanceErrorMessage.CardNotFound);

            var categories = await categoryRepository.GetAllAsync();
            var categoryList = string.Join(", ", categories.Select(c => $"{c.Name} (ID:{c.Id})"));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64Pdf = Convert.ToBase64String(ms.ToArray());

            var prompt = $@"
                Aja como um especialista financeiro analisando faturas de cartão de crédito em PDF.

                Categorias existentes no sistema:
                [{categoryList}]

                Objetivo:
                Extrair corretamente as transações financeiras.

                Regras:
                1. Extraia:
                - TransactionDate
                - Description
                - Amount

                2. Ignore pagamentos de fatura, estornos, créditos ou ajustes.

                3. Categorias:
                - Se houver similaridade, use CategoryId
                - Caso contrário, sugira NewCategoryName

                4. Parcelamentos:
                - Identifique 'Parcela X de Y'

                5. Datas:
                - Transações parceladas devem pertencer ao mês da fatura

                Formato:
                Retorne APENAS um array JSON válido:
                [
                  {{
                    ""Description"": ""string"",
                    ""Amount"": number,
                    ""TransactionDate"": ""YYYY-MM-DD"",
                    ""CategoryId"": ""uuid?"",
                    ""NewCategoryName"": ""string?""
                  }}
                ]";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = prompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "application/pdf",
                                    data = base64Pdf
                                }
                            }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent",
                requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                logger.LogError(
                    "Gemini API Error: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorBody);

                return Result.Fail(FinanceErrorMessage.InternalServerError);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var aiItems = ParseGeminiResponse(responseBody);

            if (!aiItems.Any())
                return Result.Ok(0);

            Invoice? invoice = null;
            var transactions = new List<Domain.Entities.Transaction>();

            foreach (var item in aiItems)
            {
                var isDuplicate = await transactionRepository.ExistsDuplicateAsync(
                    userId,
                    item.Amount,
                    item.TransactionDate,
                    item.Description);

                if (isDuplicate)
                    continue;

                Guid categoryId;

                if (!string.IsNullOrEmpty(item.CategoryId)
                    && Guid.TryParse(item.CategoryId, out var parsedCategoryId))
                {
                    categoryId = parsedCategoryId;
                }
                else
                {
                    var name = item.NewCategoryName ?? "Outros";
                    var existingCategory = await categoryRepository.GetByNameAsync(name);

                    if (existingCategory != null)
                    {
                        categoryId = existingCategory.Id;
                    }
                    else
                    {
                        var newCategory = new Domain.Entities.Category(name);
                        await categoryRepository.InsertAsync(newCategory);
                        categoryId = newCategory.Id;
                    }
                }

                var referenceDate = ResolveInvoiceReferenceDate(
                    item.TransactionDate,
                    card.ClosingDay);

                invoice ??= await invoiceRepository
                    .GetByCardAndDateAsync(card.Id, referenceDate);

                if (invoice is null)
                {
                    invoice = new Invoice(
                        card.Id,
                        referenceDate,
                        new DateTime(referenceDate.Year, referenceDate.Month, card.DueDay));

                    await invoiceRepository.InsertAsync(invoice);
                }

                invoice.AddTransactionAmount(item.Amount);

                var transaction = new Domain.Entities.Transaction(
                    userId,
                    categoryId,
                    item.Amount,
                    item.TransactionDate,
                    TransactionType.EXPENSE,
                    card.Id,
                    PaymentMethod.CREDIT,
                    invoice.Id,
                    1,
                    1,
                    false,
                    false,
                    null,
                    item.Description
                );

                transactions.Add(transaction);
            }

            if (!transactions.Any())
                return Result.Ok(0);

            var success = await transactionRepository.InsertBulkAsync(transactions);

            logger.LogInformation(
                "ImportInvoice finished successfully - Imported: {Count}",
                transactions.Count);

            return success
                ? Result.Ok(transactions.Count)
                : Result.Fail(FinanceErrorMessage.DatabaseError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Critical error during invoice import | UserId: {UserId}",
                userId);

            return Result.Fail(FinanceErrorMessage.UnexpectedError);
        }
    }

    private static DateTime ResolveInvoiceReferenceDate(
        DateTime transactionDate,
        int closingDay)
    {
        if (transactionDate.Day > closingDay)
            transactionDate = transactionDate.AddMonths(1);

        return new DateTime(transactionDate.Year, transactionDate.Month, 1);
    }

    private static HttpClient CreateHttpClient(IConfiguration configuration)
    {
        var apiKey = configuration["Gemini:ApiKey"]
            ?? throw new ArgumentException("Gemini ApiKey missing no appsettings.json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
        return client;
    }

    private static List<AIInvoiceItemDto> ParseGeminiResponse(string response)
    {
        using var doc = JsonDocument.Parse(response);
        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        var cleanJson = text?
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        return JsonSerializer.Deserialize<List<AIInvoiceItemDto>>(
            cleanJson!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? new();
    }
}
