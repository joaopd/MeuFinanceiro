using Domain.Enums;

namespace Application.Services.Transaction.GetTransactionsByPeriod;

public sealed class GetTransactionsByPeriodRequest
{
    public Guid UserId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public TransactionType? TransactionType { get; init; }

    public int CurrentPage { get; init; } = 1;
    public int RowsPerPage { get; init; } = 10;

    public string? OrderBy { get; init; }
    public bool OrderAsc { get; init; } = false;
}