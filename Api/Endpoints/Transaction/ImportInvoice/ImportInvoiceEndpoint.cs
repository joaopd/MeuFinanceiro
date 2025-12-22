using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.ImportInvoice;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.ImportInvoice;

public class ImportInvoiceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/transactions/import-invoice", HandleAsync)
            .WithTags("Transactions")
            .WithName("ImportInvoice")
            .WithSummary("Importar fatura PDF via IA")
            .WithDescription("Lê um ficheiro PDF de fatura, identifica gastos e categorias via Gemini e insere no sistema.")
            .DisableAntiforgery()
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] Guid cardId,
        [FromForm] Guid userId,
        IFormFile file, 
        [FromServices] IImportInvoiceService service)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("Arquivo não recebido ou vazio. Verifique se a chave no form-data é 'file'.");

        var isPdf = file.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) || 
                    file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

        if (!isPdf)
            return Results.BadRequest("Apenas arquivos PDF são aceitos.");

        var result = await service.ExecuteAsync(userId, cardId, file);

        return result.IsSuccess ? Results.Ok(new { Count = result.Value }) : result.ToProblemDetails();
    }
}