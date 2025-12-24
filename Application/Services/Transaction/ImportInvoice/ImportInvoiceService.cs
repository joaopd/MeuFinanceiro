using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                Extraia transações financeiras de uma fatura de cartão de crédito em PDF.

                Ignore:
                - pagamentos de fatura
                - estornos
                - créditos
                - ajustes

                Extraia apenas:
                - Description
                - Amount
                - TransactionDate

                Categorias disponíveis:
                [{{categoryList}}]

                Categorias:
                - Se houver similaridade, retorne CategoryId
                - Caso contrário, retorne NewCategoryName

                Parcelamentos:
                - Identifique quando a descrição contiver ""Parcela X de Y""

                Formato de resposta:
                Retorne APENAS um array JSON válido, sem explicações:

                [
                  {{
                    ""Description"": ""string"",
                    ""Amount"": number,
                    ""TransactionDate"": ""YYYY-MM-DD"",
                    ""CategoryId"": ""uuid?"",
                    ""NewCategoryName"": ""string?""
                  }}
                ]
                ";
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

            var invoices = new Dictionary<DateTime, Invoice>();
            var transactions = new List<Domain.Entities.Transaction>();

            foreach (var item in aiItems)
            {
                var effectiveTransactionDate =
                    ResolveEffectiveTransactionDate(item.TransactionDate, item.Description);

                var isDuplicate = await transactionRepository.ExistsDuplicateAsync(
                    userId,
                    item.Amount,
                    effectiveTransactionDate,
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
                        categoryId = existingCategory.Id;
                    else
                    {
                        var newCategory = new Domain.Entities.Category(name);
                        await categoryRepository.InsertAsync(newCategory);
                        categoryId = newCategory.Id;
                    }
                }

                var referenceDate = ResolveInvoiceReferenceDate(
                    effectiveTransactionDate,
                    card.ClosingDay);

                if (!invoices.TryGetValue(referenceDate, out var invoice))
                {
                    invoice = await invoiceRepository
                        .GetByCardAndDateAsync(card.Id, referenceDate);

                    if (invoice is null)
                    {
                        invoice = new Invoice(
                            card.Id,
                            referenceDate,
                            new DateTime(referenceDate.Year, referenceDate.Month, card.DueDay));

                        await invoiceRepository.InsertAsync(invoice);
                    }

                    invoices[referenceDate] = invoice;
                }

                invoice.AddTransactionAmount(item.Amount);

                var transaction = new Domain.Entities.Transaction(
                    userId,
                    categoryId,
                    item.Amount,
                    effectiveTransactionDate,
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
            if (!success)
                return Result.Fail(FinanceErrorMessage.DatabaseError);

            foreach (var invoice in invoices.Values)
            {
                logger.LogInformation(
                    "Updating Invoice {InvoiceId} - TotalAmount: {Total}",
                    invoice.Id,
                    invoice.TotalAmount);

                await invoiceRepository.UpdateAsync(invoice);
            }

            logger.LogInformation(
                "ImportInvoice finished successfully - Imported: {Count}",
                transactions.Count);

            return Result.Ok(transactions.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Critical error during invoice import | UserId: {UserId}",
                userId);

            return Result.Fail(FinanceErrorMessage.UnexpectedError);
        }
    }

    private static DateTime ResolveEffectiveTransactionDate(
        DateTime originalDate,
        string description)
    {
        if (!TryParseInstallment(description, out var current))
            return originalDate;

        return originalDate.AddMonths(current - 1);
    }

    private static bool TryParseInstallment(string description, out int current)
    {
        current = 0;

        var match = Regex.Match(
            description,
            @"Parcela\s+(\d+)\s+de\s+(\d+)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
            return false;

        current = int.Parse(match.Groups[1].Value);
        return true;
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
