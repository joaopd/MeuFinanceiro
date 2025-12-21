using Domain.Enums;

namespace Application.Services.Transaction.UpdateTransaction;

public class UpdateTransactionRequestDto
{
    public Guid TransactionId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public Guid UpdatedBy { get; set; }
}