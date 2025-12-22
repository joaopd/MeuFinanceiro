namespace Infra.Queries;

public static class InvoiceQueries
{
    public const string GetByCardAndDate = @"
        SELECT * FROM Invoices 
        WHERE CardId = @CardId 
        AND MONTH(ReferenceDate) = @Month 
        AND YEAR(ReferenceDate) = @Year";

    public const string GetByUserId = @"
        SELECT i.* FROM Invoices i 
        INNER JOIN Cards c ON i.CardId = c.Id 
        WHERE c.UserId = @UserId 
        ORDER BY i.DueDate DESC";

    public const string Insert = @"
        INSERT INTO Invoices (Id, CardId, ReferenceDate, DueDate, TotalAmount, IsPaid, CreatedAt, CreatedBy) 
        VALUES (@Id, @CardId, @ReferenceDate, @DueDate, @TotalAmount, @IsPaid, @CreatedAt, @CreatedBy)";

    public const string GetById = "SELECT * FROM Invoices WHERE Id = @Id";

    public const string PayInvoice = @"
        UPDATE Invoices 
        SET IsPaid = 1, UpdatedAt = GETDATE(), UpdatedBy = @UserId 
        WHERE Id = @InvoiceId";

    public const string PayRelatedTransactions = @"
        UPDATE Transactions 
        SET IsPaid = 1, UpdatedAt = GETDATE(), UpdatedBy = @UserId 
        WHERE InvoiceId = @InvoiceId";
    

        public const string GetAll = @"
        SELECT * FROM Invoices
        ORDER BY DueDate DESC";

        public const string Update = @"
        UPDATE Invoices SET
            ReferenceDate = @ReferenceDate,
            DueDate = @DueDate,
            TotalAmount = @TotalAmount,
            IsPaid = @IsPaid,
            UpdatedAt = @UpdatedAt,
            UpdatedBy = @UpdatedBy
        WHERE Id = @Id";

        public const string Delete = @"
        DELETE FROM Invoices WHERE Id = @Id";
        
}