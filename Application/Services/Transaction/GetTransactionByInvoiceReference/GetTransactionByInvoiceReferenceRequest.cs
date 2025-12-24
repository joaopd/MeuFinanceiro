namespace Application.Services.Transaction.GetTransactionByInvoiceReference;

public record GetTransactionByInvoiceReferenceRequest( Guid UserId, Guid CardId, DateTime InvoiceReference);