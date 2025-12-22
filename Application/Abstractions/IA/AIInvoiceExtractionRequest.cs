namespace Application.Abstractions.IA;

public record AiInvoiceExtractionRequest(
    string Base64Pdf,
    string CategoriesContext
);

public record AiInvoiceExtractionItem(
    string Description,
    decimal Amount,
    DateTime TransactionDate,
    string? CategoryId,
    string? NewCategoryName
);