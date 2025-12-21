using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class CardMapper
{
    public static CardResponseDto ToDto(this Card entity)
    {
        return new CardResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PaymentMethod = entity.PaymentMethod,
            CreditLimit = entity.CreditLimit
        };
    }
}