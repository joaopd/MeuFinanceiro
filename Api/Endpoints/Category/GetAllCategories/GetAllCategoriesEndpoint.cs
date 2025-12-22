using Api.Abstractions;
using Api.Extensions;
using Application.Services.Category.GetAllCategories;
using Application.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Category.GetAllCategories;

public class GetAllCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/categories", HandleAsync)
            .WithTags("Categories")
            .WithName("GetAllCategories")
            .WithSummary("Listar todas as categorias")
            .WithDescription("Retorna a lista de todas as categorias cadastradas.")
            .Produces<IEnumerable<CategoryResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IGetAllCategoriesService service)
    {
        var result = await service.ExecuteAsync();

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok(result.Value);
    }
}