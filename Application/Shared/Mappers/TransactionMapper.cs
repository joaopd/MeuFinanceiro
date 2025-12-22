using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class TransactionMapper
{
    public static TransactionResponseDto ToDto(this Transaction transaction)
    {
        return new TransactionResponseDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            CardId = transaction.CardId,
            PaymentMethod = transaction.PaymentMethod,
            IsFixed = transaction.IsFixed,
            Observation = transaction.Observation,
            IsPaid = transaction.IsPaid
        };
    }
}