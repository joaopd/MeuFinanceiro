using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Card.UpdateCard;

public interface IUpdateCardService
{
    Task<Result<CardResponseDto>> ExecuteAsync(UpdateCardRequestDto request);
}