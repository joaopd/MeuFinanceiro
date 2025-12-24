using Application.Services.Transaction.GetTransactionsByPeriod;
using Application.Shared.Dtos;
using Application.Shared.Results;
using FluentResults;

namespace Application.Services.Transaction.GetTransactionByInvoiceReference;

public interface IGetTransactionByInvoiceReferenceService
{
    Task<Result<List<TransactionResponseDto>>> ExecuteAsync(
        GetTransactionByInvoiceReferenceRequest request);
}
