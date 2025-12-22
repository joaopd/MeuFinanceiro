using Api.Abstractions;
using Api.Extensions;
using Application.Services.Card.GetFamilyCards;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Card.GetFamilyCards;

public class GetFamilyCardsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/cards", HandleAsync)
            .WithTags("Cards")
            .WithName("GetFamilyCards")
            .WithSummary("Lista cartões do usuário e dependentes")
            .Produces<IEnumerable<CardResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] Guid userId,
        [FromServices] IGetFamilyCardsService service)
    {
        var result = await service.ExecuteAsync(userId);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}