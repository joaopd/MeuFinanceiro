using Api.Abstractions;
using Api.Extensions;
using Application.Services.Card.GetUserCards;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Card.GetUserCards;

public class GetUserCardsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/cards/{userId:guid}", HandleAsync)
            .WithTags("Cards")
            .WithName("GetUserCards")
            .WithSummary("Lista cartões de um usuário específico")
            .Produces<IEnumerable<CardResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid userId,
        [FromServices] IGetUserCardsService service)
    {
        var result = await service.ExecuteAsync(userId);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}