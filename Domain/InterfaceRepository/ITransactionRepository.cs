using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository.BaseRepository;
using Domain.Records;

namespace Domain.InterfaceRepository;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByUserAndPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate
    );

    Task<IEnumerable<TransactionPagedRow>> GetByUserAndPeriodPagedAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        TransactionType? transactionType,
        int currentPage,
        int rowsPerPage,
        string? orderBy,
        bool orderAsc,
        bool includeDependents = false,
        Guid? cardId = null
    );

    Task<decimal> GetPaidExpensesAmountAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate
    );


    Task<decimal> GetBalanceByPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate
    );
    
    Task<bool> InsertBulkAsync(IEnumerable<Transaction> entities);
    
    Task<IEnumerable<CategoryExpenseRecord>> GetExpensesByCategoryAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate
    );
    
    Task<IEnumerable<CashFlowRecord>> GetCashFlowAsync(Guid userId, DateTime start, DateTime end);
    Task<bool> ExistsByFixedExpenseAsync(Guid fixedExpenseId, int month, int year);
    Task<decimal> GetCreditCardInvoiceSumAsync(Guid cardId, DateTime startDate, DateTime endDate);
}
