using Api.Abstractions;
using Api.Extensions;
using Application.Services.User.GetDependents;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.User.GetDependents;

public class GetDependentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/{userId:guid}/dependents", HandleAsync)
            .WithTags("Users")
            .WithName("GetDependents")
            .WithSummary("Lista dependentes")
            .Produces<List<UserResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid userId,
        [FromServices] IGetDependentsService service)
    {
        var result = await service.ExecuteAsync(userId);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}