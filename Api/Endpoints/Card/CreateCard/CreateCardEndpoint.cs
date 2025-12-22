using Api.Abstractions;
using Api.Extensions;
using Application.Services.Card.CreateCard;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Card.CreateCard;

public class CreateCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/cards", HandleAsync)
            .WithTags("Cards")
            .WithName("CreateCard")
            .WithSummary("Cria um novo cartão de crédito")
            .Produces<CardResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCardRequestDto request,
        [FromServices] ICreateCardService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
        
    }
}