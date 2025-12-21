using Domain.Entities;
using Domain.Records;

namespace Application.Shared.Mappers;

public static class TransactionPagedRowMapper
{
    public static Transaction ToTransaction(this TransactionPagedRow row)
    {
        return new Transaction(
            row.UserId,
            row.CategoryId,
            row.Amount,
            row.TransactionDate,
            row.TransactionType,
            row.CardId,
            row.PaymentMethod,
            row.InstallmentNumber,
            row.TotalInstallments,
            row.IsFixed,
            row.IsPaid,
            null,
            row.Observation
        );
    }
}