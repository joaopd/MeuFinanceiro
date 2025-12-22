using Api.Abstractions;
using Api.Extensions;
using Application.Services.User.GetByEmail;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.User.GetByEmail;

public class GetByEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/login", HandleAsync)
            .WithTags("Users")
            .WithName("GetByEmail")
            .WithSummary("Login (Buscar por E-mail)")
            .Produces<UserResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] string email,
        [FromServices] IGetByEmailService service)
    {
        var result = await service.ExecuteAsync(email);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}