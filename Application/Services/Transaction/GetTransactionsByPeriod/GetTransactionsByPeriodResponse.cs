using Domain.Enums;

namespace Application.Services.Transaction.GetTransactionsByPeriod;

public class GetTransactionsByPeriodResponse
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public TransactionType TransactionType { get; init; }
    public Guid CategoryId { get; init; }
    public Guid? CardId { get; init; }
    public DateTime TransactionDate { get; init; }
}