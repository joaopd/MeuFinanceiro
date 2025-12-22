using Domain.Abstractions.ErrorHandling;
using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.InterfaceRepository.BaseRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.CreateTransaction;

public class CreateTransactionService(
    ITransactionRepository transactionRepository,
    IInvoiceRepository invoiceRepository,
    ICardRepository cardRepository,
    ILogger<CreateTransactionService> logger) : ICreateTransactionService
{
   public async Task<Result<bool>> ExecuteAsync(CreateTransactionRequestDto request)
    {
        try
        {
            if (request.Amount <= 0)
                return Result.Fail(FinanceErrorMessage.InvalidTransactionAmount);

            if (request.CardId is not null && request.PaymentMethod is null)
                return Result.Fail(FinanceErrorMessage.PaymentMethodRequiredForCardTransaction);

            if (request.TotalInstallments < 1)
                request.TotalInstallments = 1;

            Invoice? invoice = null;
            Domain.Entities.Card? card = null;

            if (request.PaymentMethod == PaymentMethod.CREDIT)
            {
                card = await cardRepository.GetByIdAsync(request.CardId!.Value);
                if (card is null)
                    return Result.Fail(FinanceErrorMessage.CardNotFound);
            }

            var transactions = new List<Domain.Entities.Transaction>();
            for (int i = 0; i < request.TotalInstallments; i++)
            {
                var transactionDate = request.TransactionDate.AddMonths(i);
                Guid? invoiceId = null;

                if (request.PaymentMethod == PaymentMethod.CREDIT)
                {
                    var referenceDate = ResolveInvoiceReferenceDate(
                        transactionDate,
                        card!.ClosingDay);

                    invoice = await invoiceRepository.GetByCardAndDateAsync(
                        card.Id,
                        referenceDate);

                    if (invoice is null)
                    {
                        invoice = new Invoice(
                            card.Id,
                            referenceDate,
                            new DateTime(
                                referenceDate.Year,
                                referenceDate.Month,
                                card.DueDay));

                        await invoiceRepository.InsertAsync(invoice);
                    }

                    invoice.AddTransactionAmount(request.Amount);
                    invoiceId = invoice.Id;
                }

                var transaction = new Domain.Entities.Transaction(
                    request.UserId,
                    request.CategoryId,
                    request.Amount,
                    transactionDate,
                    request.TransactionType,
                    request.CardId,
                    request.PaymentMethod,
                    invoiceId,              
                    i + 1,                 
                    request.TotalInstallments,
                    request.IsFixed, 
                    false,                 
                    null,
                    request.Observation
                );

                transactions.Add(transaction);
            }

            await transactionRepository.InsertBulkAsync(transactions);

            if (invoice is not null)
                await invoiceRepository.UpdateAsync(invoice);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while creating transaction");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }

    private static DateTime ResolveInvoiceReferenceDate(
        DateTime transactionDate,
        int closingDay)
    {
        if (transactionDate.Day > closingDay)
            transactionDate = transactionDate.AddMonths(1);

        return new DateTime(transactionDate.Year, transactionDate.Month, 1);
    }
}
