using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository.BaseRepository; // Certifique-se que está usando o repositório correto
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.GetUserCards;

public class GetUserCardsService : IGetUserCardsService
{
    private readonly ICardRepository _cardRepository;
    private readonly ILogger<GetUserCardsService> _logger;

    public GetUserCardsService(
        ICardRepository cardRepository,
        ILogger<GetUserCardsService> logger)
    {
        _cardRepository = cardRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(Guid userId, bool includeDependents)
    {
        try
        {
            _logger.LogInformation(
                "GetUserCards started - TargetUserId: {UserId}, IncludeDependents: {IncludeDependents}", 
                userId, includeDependents);

            var cards = includeDependents 
                ? await _cardRepository.GetFamilyCardsAsync(userId)
                : await _cardRepository.GetByUserIdAsync(userId);

            _logger.LogInformation("GetUserCards finished - Count: {Count}", cards.Count());

            return Result.Ok(cards.Select(c => c.ToDto()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user cards for UserId: {UserId}", userId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}