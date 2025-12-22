using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.DeleteCard;

public class DeleteCardService(
    ICardRepository cardRepository,
    ILogger<DeleteCardService> logger)
    : IDeleteCardService
{
    public async Task<Result> ExecuteAsync(Guid id)
    {
        try
        {
            logger.LogInformation("DeleteCard started - CardId: {CardId}", id);

            var card = await cardRepository.GetByIdAsync(id);

            if (card is null)
            {
                logger.LogWarning("DeleteCard failed - Card not found: {CardId}", id);
                return Result.Fail(FinanceErrorMessage.CardNotFound);
            }

            // Exclusão Lógica
            card.Delete();

            await cardRepository.UpdateAsync(card);

            logger.LogInformation("DeleteCard finished successfully - CardId: {CardId}", id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting card: {CardId}", id);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}