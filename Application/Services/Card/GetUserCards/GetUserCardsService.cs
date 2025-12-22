using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.Entities; 
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.GetUserCards;

public class GetUserCardsService : IGetUserCardsService
{
    private readonly ICardRepository _cardRepository; 
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GetUserCardsService> _logger;

    public GetUserCardsService(
        ICardRepository cardRepository, ITransactionRepository transactionRepository,
        ILogger<GetUserCardsService> logger)
    {
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(Guid userId, bool includeDependents)
    {
        try
        {
            var cards = includeDependents 
                ? await _cardRepository.GetFamilyCardsAsync(userId)
                : await _cardRepository.GetByUserIdAsync(userId);

            var cardDtos = new List<CardResponseDto>(); 
            
            var today = DateTime.UtcNow.Date;
            
            foreach (var card in cards)
            {
                
                DateTime invoiceStart, invoiceEnd;
                
                var closingThisMonth = new DateTime(today.Year, today.Month, 
                    Math.Min(card.ClosingDay, DateTime.DaysInMonth(today.Year, today.Month)));

                if (today >= closingThisMonth)
                {
                    invoiceStart = closingThisMonth; 
                    invoiceEnd = closingThisMonth.AddMonths(1);
                }
                else
                {
                    invoiceStart = closingThisMonth.AddMonths(-1);
                    invoiceEnd = closingThisMonth;
                }
                
                var debt = await _transactionRepository.GetCreditCardInvoiceSumAsync(card.Id, invoiceStart, invoiceEnd);

                cardDtos.Add(new CardResponseDto(
                    card.Id,
                    card.Name,
                    card.CreditLimit,
                    debt,
                    card.UserId,
                    card.ClosingDay,
                    card.DueDay,
                    card.Color
                ));
            }

            return Result.Ok((IEnumerable<CardResponseDto>)cardDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user cards");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}