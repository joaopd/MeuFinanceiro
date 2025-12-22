using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.GetFamilyCards;

public class GetFamilyCardsService(
    ICardRepository cardRepository,
    ITransactionRepository transactionRepository,
    ILogger<GetFamilyCardsService> logger)
    : IGetFamilyCardsService
{
    public async Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(Guid userId)
    {
        try
        {
            logger.LogInformation("GetFamilyCards started - UserId: {UserId}", userId);

            var cards = await cardRepository.GetFamilyCardsAsync(userId);

            logger.LogInformation("GetFamilyCards finished - Count: {Count}", cards.Count());
            
            var today = DateTime.UtcNow.Date;
            
            var cardDtos = new List<CardResponseDto>();
            
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
                
                var debt = await transactionRepository.GetCreditCardInvoiceSumAsync(card.Id, invoiceStart, invoiceEnd);

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
            logger.LogError(ex, "Error fetching family cards for UserId: {UserId}", userId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}