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
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Cria uma nova transação",
                Description = "Cria uma transação financeira (Despesa ou Receita), suportando parcelamento."
            });
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