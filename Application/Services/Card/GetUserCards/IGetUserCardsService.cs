using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Card.GetUserCards;

public interface IGetUserCardsService
{
    Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(Guid userId);
}