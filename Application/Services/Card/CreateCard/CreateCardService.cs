using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.CreateCard;

public class CreateCardService(
    ICardRepository cardRepository,
    ILogger<CreateCardService> logger)
    : ICreateCardService
{
    public async Task<Result<CardResponseDto>> ExecuteAsync(CreateCardRequestDto request)
    {
        try
        {
            logger.LogInformation("CreateCard started - Name: {Name}", request.Name);

            string color = string.IsNullOrWhiteSpace(request.Color) ? "#000000" : request.Color;

            var card = new Domain.Entities.Card(
                request.Name,
                request.CreditLimit,
                request.UserId,
                request.ClosingDay,
                request.DueDay,
                color
            );

            await cardRepository.AddAsync(card);
        
            logger.LogInformation("CreateCard finished - Id: {Id}", card.Id);
            return Result.Ok(card.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating card");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}