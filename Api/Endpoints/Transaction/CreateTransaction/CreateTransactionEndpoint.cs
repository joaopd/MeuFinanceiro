using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.CreateTransaction;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.CreateTransaction;

public class CreateTransactionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/transactions", HandleAsync)
            .WithTags("Transactions")
            .WithName("CreateTransaction")
            .WithSummary("Criar nova transação")
            .WithDescription("Registra uma nova despesa ou receita.")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateTransactionRequestDto request,
        [FromServices] ICreateTransactionService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
        {
            return result.ToProblemDetails(); 
        }

        return Results.Created();
    }
}