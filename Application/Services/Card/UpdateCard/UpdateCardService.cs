using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.UpdateCard;

public class UpdateCardService : IUpdateCardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ILogger<UpdateCardService> _logger;

    public UpdateCardService(
        ICardRepository cardRepository,
        ILogger<UpdateCardService> logger)
    {
        _cardRepository = cardRepository;
        _logger = logger;
    }

    public async Task<Result<CardResponseDto>> ExecuteAsync(UpdateCardRequestDto request)
    {
        try
        {
            _logger.LogInformation("UpdateCard started - CardId: {CardId}", request.Id);

            var card = await _cardRepository.GetByIdAsync(request.Id);

            if (card is null)
            {
                _logger.LogWarning("UpdateCard failed - Card not found: {CardId}", request.Id);
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

            await _cardRepository.UpdateAsync(card);

            _logger.LogInformation("UpdateCard finished successfully - CardId: {CardId}", request.Id);

            return Result.Ok(card.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card: {CardId}", request.Id);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}