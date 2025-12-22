using Api.Abstractions;
using Api.Extensions;
using Application.Services.FixedExpense.UpdateFixedExpense;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.FixedExpense.UpdateFixedExpense;

public class UpdateFixedExpenseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/fixed-expenses", HandleAsync)
            .WithTags("Fixed Expenses")
            .WithName("UpdateFixedExpense")
            .WithSummary("Atualizar uma despesa fixa existente")
            .Produces<FixedExpenseResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateFixedExpenseRequestDto request,
        [FromServices] IUpdateFixedExpenseService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}