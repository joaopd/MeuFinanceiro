using Api.Abstractions;
using Api.Extensions;
using Application.Services.Card.UpdateCard;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Card.UpdateCard;

public class UpdateCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/cards", HandleAsync)
            .WithTags("Cards")
            .WithName("UpdateCard")
            .WithSummary("Atualiza os dados de um cartão")
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateCardRequestDto request,
        [FromServices] IUpdateCardService service)
    {
        var result = await service.ExecuteAsync(request);
        
        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}