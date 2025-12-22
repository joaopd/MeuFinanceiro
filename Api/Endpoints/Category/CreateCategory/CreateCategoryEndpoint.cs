using Api.Abstractions;
using Api.Extensions;
using Application.Services.Category.CreateCategory;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Category.CreateCategory;

public class CreateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/categories", HandleAsync)
            .WithTags("Categories")
            .WithName("CreateCategory")
            .WithSummary("Criar nova categoria")
            .WithDescription("Cadastra uma nova categoria de Receita ou Despesa.")
            .Produces<CategoryResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCategoryRequestDto request,
        [FromServices] ICreateCategoryService service)
    {
        var result = await service.ExecuteAsync(request);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Created($"/api/v1/categories/{result.Value.Id}", result.Value);
    }
}