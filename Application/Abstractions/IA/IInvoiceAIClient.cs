namespace Application.Abstractions.IA;

public interface IInvoiceAiClient
{
    Task<IReadOnlyList<AiInvoiceExtractionItem>> ExtractInvoiceAsync(
        AiInvoiceExtractionRequest request,
        CancellationToken cancellationToken = default);
}