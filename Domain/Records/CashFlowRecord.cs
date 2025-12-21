using Domain.Enums;

namespace Domain.Records;

public class CashFlowRecord
{
    public TransactionType TransactionType { get; set; }
    public decimal Total { get; set; }
}