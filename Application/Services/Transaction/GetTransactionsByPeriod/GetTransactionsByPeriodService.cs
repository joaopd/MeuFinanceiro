using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Application.Shared.Results;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.GetTransactionsByPeriod;

public sealed class  GetTransactionsByPeriodService(
    ITransactionRepository transactionRepository,
    ILogger<GetTransactionsByPeriodService> logger)
    : IGetTransactionsByPeriodService
{
    public async Task<Result<PaginatedResult<TransactionResponseDto>>> ExecuteAsync(
        GetTransactionsByPeriodRequest request)
    {
        try
        {
            logger.LogInformation(
                "GetTransactionsByPeriod started | UserId: {UserId} | Period: {Start} - {End} | CardId: {CardId}",
                request.UserId,
                request.StartDate,
                request.EndDate,
                request.CardId);

            if (request.StartDate > request.EndDate)
                return Result.Fail(FinanceErrorMessage.InvalidPeriod);

            var rows = (await transactionRepository
                    .GetByUserAndPeriodPagedAsync(
                        request.UserId,
                        request.StartDate,
                        request.EndDate,
                        request.TransactionType,
                        request.CurrentPage,
                        request.RowsPerPage,
                        request.OrderBy,
                        request.OrderAsc,
                        request.IncludeDependents,
                        cardId: request.CardId
                    ))
                .ToList();

            if (!rows.Any())
            {
                logger.LogInformation(
                    "GetTransactionsByPeriod finished | No transactions found");

                return Result.Ok(PaginatedResult<TransactionResponseDto>.Empty());
            }

            var meta = rows.First();

            var result = new PaginatedResult<TransactionResponseDto>
            {
                TotalRows = meta.TotalRows,
                TotalPages = meta.TotalPages,
                CurrentPage = meta.CurrentPage,
                RowsPerPage = meta.RowsPerPage,
                Data = rows
                    .Select(r =>
                    {
                        var transaction = r.ToTransaction();
                        return transaction.ToDto(r.CategoryName);
                    })
                    .ToList()
            };

            logger.LogInformation(
                "GetTransactionsByPeriod finished successfully | TotalRows: {TotalRows} | Paid: {PaidCount} | WithInvoice: {InvoiceCount}",
                result.TotalRows,
                result.Data.Count(t => t.IsPaid),
                result.Data.Count(t => t.InvoiceId.HasValue));

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while getting transactions by period | UserId: {UserId}",
                request.UserId);

            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}
