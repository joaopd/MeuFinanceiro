using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.TogglePaymentStatus;

public interface ITogglePaymentStatusService
{
    Task<Result> ExecuteAsync(Guid transactionId, bool isPaid, Guid updatedBy);
}