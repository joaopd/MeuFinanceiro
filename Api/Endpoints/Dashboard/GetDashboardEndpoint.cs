using Api.Abstractions;
using Api.Extensions;
using Application.Services.Dashboard.GetDashboardService;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Dashboard.GetDashboard;

public class GetDashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/dashboard", HandleAsync)
            .WithTags("Dashboard")
            .WithName("GetDashboard")
            .WithSummary("Obter indicadores do dashboard")
            .WithDescription("Retorna totais de entradas, saídas, saldo e gráficos por categoria para um período específico.")
            .Produces<GetDashboardResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] Guid userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromServices] IGetDashboardService service)
    {
        var result = await service.ExecuteAsync(userId, startDate, endDate);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}