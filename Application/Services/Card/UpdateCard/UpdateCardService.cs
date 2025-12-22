using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;

namespace Application.Services.Card.UpdateCard;

public class UpdateCardService : IUpdateCardService
{
    private readonly ICardRepository _cardRepository;

    public UpdateCardService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result<CardResponseDto>> ExecuteAsync(UpdateCardRequestDto request)
    {
        var card = await _cardRepository.GetByIdAsync(request.Id);

        if (card is null)
            return Result.Fail(FinanceErrorMessage.CardNotFound);

        card.Update(request.Name, request.CreditLimit, request.ClosingDay, request.DueDay, request.Color);

        await _cardRepository.UpdateAsync(card);

        return Result.Ok(card.ToDto());
    }
}