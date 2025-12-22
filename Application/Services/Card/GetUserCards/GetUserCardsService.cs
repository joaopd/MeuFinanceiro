using Application.Shared.Dtos;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.GetUserCards;

public class GetUserCardsService(
    ICardRepository cardRepository,
    IInvoiceRepository invoiceRepository,
    ILogger<GetUserCardsService> logger)
    : IGetUserCardsService
{
    public async Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(
        Guid userId,
        bool includeDependents)
    {
        try
        {
            var cards = includeDependents
                ? await cardRepository.GetFamilyCardsAsync(userId)
                : await cardRepository.GetByUserIdAsync(userId);

            var today = DateTime.UtcNow.Date;
            var result = new List<CardResponseDto>();

            foreach (var card in cards)
            {
                var referenceDate = ResolveInvoiceReferenceDate(today, card.ClosingDay);

                var invoice = await invoiceRepository
                    .GetByCardAndDateAsync(card.Id, referenceDate);

                result.Add(new CardResponseDto(
                    card.Id,
                    card.Name,
                    card.CreditLimit,
                    invoice?.TotalAmount ?? 0,
                    invoice?.IsPaid ?? false,
                    card.UserId,
                    card.ClosingDay,
                    card.DueDay,
                    card.Color
                ));
            }

            return Result.Ok((IEnumerable<CardResponseDto>)result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user cards");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }

    private static DateTime ResolveInvoiceReferenceDate(
        DateTime date,
        int closingDay)
    {
        var closingThisMonth = new DateTime(
            date.Year,
            date.Month,
            Math.Min(closingDay, DateTime.DaysInMonth(date.Year, date.Month)));

        return date >= closingThisMonth
            ? new DateTime(date.Year, date.Month, 1)
            : new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
    }
}
