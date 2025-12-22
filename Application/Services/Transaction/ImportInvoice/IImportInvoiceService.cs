using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Transaction.ImportInvoice;

public interface IImportInvoiceService
{
    Task<Result<int>> ExecuteAsync(Guid userId, Guid cardId, IFormFile file);
}