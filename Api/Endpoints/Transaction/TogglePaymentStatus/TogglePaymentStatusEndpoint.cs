using Api.Abstractions;
using Api.Extensions;
using Application.Services.Transaction.TogglePaymentStatus;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Transaction.TogglePaymentStatus;

public class TogglePaymentStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/transactions/{id:guid}/payment-status", HandleAsync)
            .WithTags("Transactions")
            .WithName("TogglePaymentStatus")
            .WithSummary("Pagar ou Estornar")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] TogglePaymentStatusRequestDto request,
        [FromServices] ITogglePaymentStatusService service)
    {
        if (id != request.TransactionId)
            return Results.BadRequest("ID mismatch");

        var result = await service.ExecuteAsync(request.TransactionId, request.IsPaid, request.UpdatedBy);

        if (result.IsFailed)
            return result.ToProblemDetails();

        return Results.Ok();
    }
}