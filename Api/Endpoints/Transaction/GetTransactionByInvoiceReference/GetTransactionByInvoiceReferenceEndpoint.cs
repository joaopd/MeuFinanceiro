using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.GetTransactionByInvoiceReference;
using Application.Shared.Dtos;
using Application.Shared.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.GetTransactionByInvoiceReference;

public class GetTransactionByInvoiceReferenceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/transactions-by-invoice", HandleAsync)
            .WithTags("Transactions")
            .WithName("GetTransactionsByInvoiceReference")
            .WithSummary("Lista transações por fatura")
            .Produces<List<TransactionResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetTransactionByInvoiceReferenceParams query, 
        [FromServices] IGetTransactionByInvoiceReferenceService service)
    {
        var request = new GetTransactionByInvoiceReferenceRequest(
            query.UserId,
            query.CardId,
            query.ReferenceDate);

        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}

public record GetTransactionByInvoiceReferenceParams(
    Guid UserId, 
    Guid CardId ,
    DateTime ReferenceDate
);

