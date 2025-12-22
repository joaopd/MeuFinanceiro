using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.FixedExpense.CreateFixedExpense;

public class CreateFixedExpenseService(
    IFixedExpenseRepository fixedExpenseRepository,
    ILogger<CreateFixedExpenseService> logger)
    : ICreateFixedExpenseService
{
 public async Task<Result<FixedExpenseResponseDto>> ExecuteAsync(CreateFixedExpenseRequestDto request)
    {
        try
        {
            logger.LogInformation(
                "CreateFixedExpense started - UserId: {UserId}, Description: {Desc}",
                request.UserId,
                request.Description);

            if (request.Amount <= 0)
            {
                return Result.Fail(FinanceErrorMessage.InvalidTransactionAmount);
            }

            var fixedExpense = new Domain.Entities.FixedExpense(
                request.UserId,
                request.Description,
                request.Amount,
                request.StartDate,
                request.Recurrence,
                request.CategoryId,
                request.CardId,
                request.EndDate
            );

            await fixedExpenseRepository.InsertAsync(fixedExpense);

            logger.LogInformation(
                "CreateFixedExpense finished successfully - Id: {Id}",
                fixedExpense.Id);

            return Result.Ok(fixedExpense.ToDto());
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data for FixedExpense");
            return Result.Fail(FinanceErrorMessage.InvalidFixedExpenseData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating fixed expense");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}