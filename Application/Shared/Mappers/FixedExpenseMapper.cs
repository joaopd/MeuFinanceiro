using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class FixedExpenseMapper
{
    public static FixedExpenseResponseDto ToDto(this FixedExpense entity)
    {
        return new FixedExpenseResponseDto
        {
            Id = entity.Id,
            Description = entity.Description,
            Amount = entity.Amount,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Recurrence = entity.Recurrence,
            CategoryId = entity.CategoryId,
            CardId = entity.CardId
        };
    }
}