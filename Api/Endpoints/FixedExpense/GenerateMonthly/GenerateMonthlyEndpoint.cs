using Api.Abstractions;
using Api.Extensions;
using Application.Services.FixedExpense.GenerateMonthly;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.FixedExpense.GenerateMonthly;

public class GenerateMonthlyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/fixed-expenses/generate", HandleAsync)
            .WithTags("Fixed Expenses")
            .WithName("GenerateMonthlyFixedExpenses")
            .WithSummary("Gera transações a partir das despesas fixas")
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
    
    private static async Task<IResult> HandleAsync(
        [FromQuery] Guid userId,
        [FromQuery] int month,
        [FromQuery] int year,
        [FromServices] IGenerateMonthlyFixedExpensesService service)
    {
        var result = await service.ExecuteAsync(userId, month, year);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(new { GeneratedCount = result.Value });
    }
}