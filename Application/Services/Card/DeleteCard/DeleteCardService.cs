using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.DeleteCard;

public class DeleteCardService : IDeleteCardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ILogger<DeleteCardService> _logger;

    public DeleteCardService(
        ICardRepository cardRepository,
        ILogger<DeleteCardService> logger)
    {
        _cardRepository = cardRepository;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("DeleteCard started - CardId: {CardId}", id);

            var card = await _cardRepository.GetByIdAsync(id);

            if (card is null)
            {
                _logger.LogWarning("DeleteCard failed - Card not found: {CardId}", id);
                return Result.Fail(FinanceErrorMessage.CardNotFound);
            }

            // Exclusão Lógica
            card.Delete();

            await _cardRepository.UpdateAsync(card);

            _logger.LogInformation("DeleteCard finished successfully - CardId: {CardId}", id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card: {CardId}", id);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}