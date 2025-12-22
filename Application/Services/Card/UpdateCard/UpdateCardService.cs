using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.UpdateCard;

public class UpdateCardService(
    ICardRepository cardRepository,
    ILogger<UpdateCardService> logger
    ) : IUpdateCardService
{

    public async Task<Result<CardResponseDto>> ExecuteAsync(UpdateCardRequestDto request)
    {
        try
        {
            logger.LogInformation("UpdateCard started - CardId: {CardId}", request.Id);

            var card = await cardRepository.GetByIdAsync(request.Id);

            if (card is null)
            {
                logger.LogWarning("UpdateCard failed - Card not found: {CardId}", request.Id);
                return Result.Fail(FinanceErrorMessage.CardNotFound);
            }

            string colorToUpdate = string.IsNullOrWhiteSpace(request.Color) ? card.Color : request.Color;

            card.Update(
                request.Name, 
                request.CreditLimit, 
                request.ClosingDay, 
                request.DueDay, 
                colorToUpdate
            );

            await cardRepository.UpdateAsync(card);

            logger.LogInformation("UpdateCard finished successfully - CardId: {CardId}", request.Id);

            return Result.Ok(card.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating card: {CardId}", request.Id);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}