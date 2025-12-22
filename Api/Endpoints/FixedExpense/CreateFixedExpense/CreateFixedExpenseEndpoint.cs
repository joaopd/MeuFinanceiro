using Api.Abstractions;
using Api.Extensions;
using Application.Services.FixedExpense.CreateFixedExpense;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.FixedExpense.CreateFixedExpense;

public class CreateFixedExpenseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/fixed-expenses", HandleAsync)
            .WithTags("Fixed Expenses")
            .WithName("CreateFixedExpense")
            .WithSummary("Cria despesa fixa")
            .WithDescription("Cadastra uma conta recorrente (Aluguel, Internet, etc).")
            .Produces<FixedExpenseResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateFixedExpenseRequestDto request,
        [FromServices] ICreateFixedExpenseService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Created($"/api/v1/fixed-expenses/{result.Value.Id}", result.Value);
    }
}