using Application.Shared.Dtos;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Card.GetFamilyCards;

public class GetFamilyCardsService(
    ICardRepository cardRepository,
    IInvoiceRepository invoiceRepository,
    ILogger<GetFamilyCardsService> logger)
    : IGetFamilyCardsService
{
    public async Task<Result<IEnumerable<CardResponseDto>>> ExecuteAsync(Guid userId)
    {
        try
        {
            logger.LogInformation(
                "GetFamilyCards started - UserId: {UserId}", userId);

            var cards = await cardRepository.GetFamilyCardsAsync(userId);
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

            logger.LogInformation(
                "GetFamilyCards finished - Count: {Count}", result.Count);

            return Result.Ok((IEnumerable<CardResponseDto>)result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error fetching family cards for UserId: {UserId}", userId);

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
