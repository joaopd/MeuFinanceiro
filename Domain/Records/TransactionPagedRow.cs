using Domain.Enums;

namespace Domain.Records;

public sealed class TransactionPagedRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid CategoryId { get; init; }
    public decimal Amount { get; init; }
    public DateTime TransactionDate { get; init; }
    public TransactionType TransactionType { get; init; }
    public Guid? CardId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public int InstallmentNumber { get; init; }
    public int TotalInstallments { get; init; }
    public bool IsFixed { get; init; }
    public bool IsPaid { get; init; }
    public int TotalRows { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
}
