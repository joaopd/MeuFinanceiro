using Api.Abstractions;
using Api.Extensions;
using Application.Services.FixedExpense.GetAllFixedExpenseByUserId;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.FixedExpense.GetAllFixedExpense;

public class GetAllFixedExpenseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/fixed-expenses", HandleAsync)
            .WithTags("Fixed Expenses")
            .WithName("GetAllFixedExpenses")
            .WithSummary("Listar todas as despesas fixas de um usuário")
            .Produces<IEnumerable<FixedExpenseResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] Guid userId,
        [FromQuery] DateTime? referenceDate,
        [FromServices] IGetAllFixedExpenseByUserIdService service)
    {
        var date = referenceDate ?? DateTime.UtcNow;
        
        var result = await service.ExecuteAsync(userId, date);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}