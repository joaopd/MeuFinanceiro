using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class CardMapper
{
    public static CardResponseDto ToDto(this Card card)
    {
        return new CardResponseDto(
            card.Id,
            card.Name,
            card.CreditLimit,
            card.UserId,
            card.ClosingDay,
            card.DueDay,
            card.Color
        );
    }
}