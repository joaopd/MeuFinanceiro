using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.GetTransactionsByPeriod;
using Application.Shared.Dtos;
using Application.Shared.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.GetTransactionsByPeriod;

public class GetTransactionsByPeriodEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/transactions", HandleAsync)
            .WithTags("Transactions")
            .WithName("GetTransactions")
            .WithSummary("Lista transações por período")
            .Produces<PaginatedResult<TransactionResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetTransactionsRequestParams query, 
        [FromServices] IGetTransactionsByPeriodService service)
    {
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = query.UserId,
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TransactionType = query.Type,
            CurrentPage = query.Page ?? 1,
            RowsPerPage = query.Rows ?? 10
        };

        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}

public record GetTransactionsRequestParams(
    Guid UserId, 
    DateTime StartDate, 
    DateTime EndDate, 
    TransactionType? Type, 
    int? Page, 
    int? Rows
);