using Api.Abstractions;
using Api.Extensions;
using Application.Services.User.CreateUser;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.User.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/users", HandleAsync)
            .WithTags("Users")
            .WithName("CreateUser")
            .WithSummary("Cria novo usuário")
            .WithDescription("Cria um usuário principal ou dependente (se ParentUserId for informado).")
            .Produces<UserResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateUserRequestDto request,
        [FromServices] ICreateUserService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Created($"/api/v1/users/{result.Value.Id}", result.Value);
    }
}