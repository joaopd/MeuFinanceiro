using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Application.Shared.Results;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.GetTransactionByInvoiceReference;

public sealed class  GetTransactionByInvoiceReferenceService(
    ITransactionRepository transactionRepository,
    ILogger<GetTransactionByInvoiceReferenceService> logger)
    : IGetTransactionByInvoiceReferenceService
{
    public async Task<Result<List<TransactionResponseDto>>> ExecuteAsync(
        GetTransactionByInvoiceReferenceRequest request)
    {
        try
        {
            logger.LogInformation(
                "GetTransactionByInvoiceReference started | UserId: {UserId} | InvoiceReference: {InvoiceReference}  | CardId: {CardId}",
                request.UserId,
                request.InvoiceReference,
                request.CardId);

            var result = await transactionRepository
                .GetAllTransactionInvoiceAsync(
                    request.UserId,
                    request.CardId,
                    request.InvoiceReference);

            return Result.Ok(result.Select(r =>
                {
                    var transaction = r.ToTransaction();
                    return transaction.ToDto(r.CategoryName);
                })
                .ToList());
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
