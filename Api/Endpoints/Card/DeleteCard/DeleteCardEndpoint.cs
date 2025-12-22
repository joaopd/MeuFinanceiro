using Api.Abstractions;
using Api.Extensions;
using Application.Services.Card.DeleteCard;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Card.DeleteCard;

public class DeleteCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/cards/{id:guid}", HandleAsync)
            .WithTags("Cards")
            .WithName("DeleteCard")
            .WithSummary("Remove (logicamente) um cartão")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IDeleteCardService service)
    {
        var result = await service.ExecuteAsync(id);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.NoContent();
    }
}