using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Application.Shared.Results;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.GetTransactionsByPeriod;

public sealed class GetTransactionsByPeriodService
    : IGetTransactionsByPeriodService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GetTransactionsByPeriodService> _logger;

    public GetTransactionsByPeriodService(
        ITransactionRepository transactionRepository,
        ILogger<GetTransactionsByPeriodService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<TransactionResponseDto>>> ExecuteAsync(
        GetTransactionsByPeriodRequest request)
    {
        try
        {
            _logger.LogInformation(
                "GetTransactionsByPeriod started | UserId: {UserId} | Period: {Start} - {End}",
                request.UserId,
                request.StartDate,
                request.EndDate);

            if (request.StartDate > request.EndDate)
                return Result.Fail(FinanceErrorMessage.InvalidPeriod);

            var rows = (await _transactionRepository
                    .GetByUserAndPeriodPagedAsync(
                        request.UserId,
                        request.StartDate,
                        request.EndDate,
                        request.TransactionType,
                        request.CurrentPage,
                        request.RowsPerPage,
                        request.OrderBy,
                        request.OrderAsc,
                        includeDependents: false,
                        cardId: request.CardId
                    ))
                .ToList();
            
            if (!rows.Any())
            {
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
                    .Select(r => r.ToTransaction().ToDto())
                    .ToList()
            };

            _logger.LogInformation(
                "GetTransactionsByPeriod finished successfully | TotalRows: {TotalRows}",
                result.TotalRows);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while getting transactions by period | UserId: {UserId}",
                request.UserId);

            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}
