using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Card.CreateCard;

public interface ICreateCardService
{
    Task<Result<CardResponseDto>> ExecuteAsync(CreateCardRequestDto request);
}