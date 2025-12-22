using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.UpdateTransaction;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.UpdateTransaction;

public class UpdateTransactionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/transactions/{id:guid}", HandleAsync)
            .WithTags("Transactions")
            .WithName("UpdateTransaction")
            .WithSummary("Atualizar transação")
            .WithDescription("Edita uma transação existente baseada no ID.")
            .Produces<TransactionResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateTransactionRequestDto request,
        [FromServices] IUpdateTransactionService service)
    {
        if (id != request.TransactionId)
            return Results.BadRequest(new ProblemDetails { Detail = "ID da rota difere do ID do corpo." });

        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}