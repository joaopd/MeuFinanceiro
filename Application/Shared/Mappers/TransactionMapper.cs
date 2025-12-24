using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class TransactionMapper
{
    public static TransactionResponseDto ToDto(this Transaction transaction, string ? categoryName = null)
    {
        return new TransactionResponseDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            CategoryName = categoryName,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            CardId = transaction.CardId,
            PaymentMethod = transaction.PaymentMethod,
            InvoiceId = transaction.InvoiceId,
            InstallmentNumber = transaction.InstallmentNumber,
            TotalInstallments = transaction.TotalInstallments,
            IsFixed = transaction.IsFixed,
            IsPaid = transaction.IsPaid,
            Observation = transaction.Observation
        };
    }
}