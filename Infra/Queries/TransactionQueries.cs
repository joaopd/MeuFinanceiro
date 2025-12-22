namespace Infra.Queries;

public static class TransactionQueries
{
    public const string GetById = """
        SELECT * FROM "Transaction" 
        WHERE "Id" = @Id AND "IsDeleted" = false;
    """;

    public const string GetAll = """
        SELECT * FROM "Transaction" 
        WHERE "IsDeleted" = false;
    """;

    public const string GetByUserAndPeriod = """
        SELECT * FROM "Transaction" 
        WHERE "UserId" = @UserId 
          AND "TransactionDate" BETWEEN @StartDate AND @EndDate
          AND "IsDeleted" = false;
    """;
    
    public const string Insert = """
                                     INSERT INTO "Transaction" (
                                         "Id", "UserId", "CategoryId", "Amount", "TransactionDate",
                                         "TransactionType", "PaymentMethod", "CardId",
                                         "InstallmentNumber", "TotalInstallments", "IsFixed", "IsPaid",
                                         "FixedExpenseId", "Observation",
                                         "CreatedAt", "UpdatedAt", "IsDeleted"
                                     )
                                     VALUES (
                                         @Id, @UserId, @CategoryId, @Amount, @TransactionDate,
                                         @TransactionType, @PaymentMethod, @CardId,
                                         @InstallmentNumber, @TotalInstallments, @IsFixed, @IsPaid,
                                         @FixedExpenseId, @Observation,
                                         @CreatedAt, @UpdatedAt, false
                                     )
                                 """;

    public const string Update = """
                                     UPDATE "Transaction"
                                     SET
                                         "Amount" = @Amount,
                                         "TransactionDate" = @TransactionDate,
                                         "Observation" = @Observation,
                                         "IsPaid" = @IsPaid,
                                         "UpdatedAt" = @UpdatedAt,
                                         "UpdatedBy" = @UpdatedBy
                                     WHERE "Id" = @Id;
                                 """;

    public const string SoftDelete = """
        UPDATE "Transaction"
        SET "IsDeleted" = true, "UpdatedAt" = @UpdatedAt
        WHERE "Id" = @Id;
    """;

    public const string GetBalanceByPeriod = """
                                                 SELECT COALESCE(SUM(
                                                     CASE 
                                                         WHEN "TransactionType" = 0 THEN "Amount" 
                                                         WHEN "TransactionType" = 1 AND "PaymentMethod" <> 0 THEN - "Amount" 
                                                         ELSE 0
                                                     END
                                                 ), 0)
                                                 FROM "Transaction"
                                                 WHERE "UserId" = @UserId
                                                   AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                                   AND "IsPaid" = true
                                                   AND "IsDeleted" = false;
                                             """;

    public const string GetCreditCardInvoiceSum = """
                                                      SELECT COALESCE(SUM("Amount"), 0)
                                                      FROM "Transaction"
                                                      WHERE "CardId" = @CardId
                                                        AND "TransactionType" = 1
                                                        AND "PaymentMethod" = 0  
                                                        AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                                        AND "IsDeleted" = false;
                                                  """;
    public const string GetByUserAndPeriodPagedWithMeta = """
                                                              WITH base AS (
                                                                  SELECT t.*, COUNT(*) OVER() AS total_rows
                                                                  FROM "Transaction" t
                                                                  WHERE t."UserId" = @UserId
                                                                    AND t."TransactionDate" BETWEEN @StartDate AND @EndDate
                                                                    AND (@TransactionType IS NULL OR t."TransactionType" = @TransactionType)
                                                                    AND (@CardId IS NULL OR t."CardId" = @CardId)
                                                                    AND t."IsDeleted" = false
                                                              ),
                                                              paged AS (
                                                                  SELECT * FROM base
                                                                  ORDER BY
                                                                      CASE WHEN @OrderBy = 'TransactionDate' AND @OrderAsc THEN "TransactionDate" END ASC,
                                                                      CASE WHEN @OrderBy = 'TransactionDate' AND NOT @OrderAsc THEN "TransactionDate" END DESC,
                                                                      CASE WHEN @OrderBy = 'Amount' AND @OrderAsc THEN "Amount" END ASC,
                                                                      CASE WHEN @OrderBy = 'Amount' AND NOT @OrderAsc THEN "Amount" END DESC,
                                                                      "TransactionDate" DESC
                                                                  LIMIT @RowsPerPage OFFSET (@CurrentPage - 1) * @RowsPerPage
                                                              )
                                                              SELECT *, total_rows,
                                                                     CEILING(total_rows::decimal / @RowsPerPage) AS total_pages,
                                                                     @CurrentPage AS current_page, @RowsPerPage AS rows_per_page
                                                              FROM paged;
                                                          """;

    public const string GetPaidExpensesAmount = """
                                                    SELECT COALESCE(SUM("Amount"), 0)
                                                    FROM "Transaction"
                                                    WHERE "UserId" = @UserId
                                                      AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                                      AND "TransactionType" = 1
                                                      AND "IsPaid" = true
                                                      AND "IsDeleted" = false;
                                                """;
    
    public const string GetExpensesByCategory = """
                                                    SELECT 
                                                        c."Name" as Category, 
                                                        SUM(t."Amount") as Total
                                                    FROM "Transaction" t  
                                                    JOIN "Category" c ON t."CategoryId" = c."Id"
                                                    WHERE t."UserId" = @UserId
                                                      AND t."TransactionDate" BETWEEN @StartDate AND @EndDate
                                                      AND t."TransactionType" = 1 
                                                      AND t."IsDeleted" = false
                                                    GROUP BY c."Name"
                                                    ORDER BY Total DESC;
                                                """;
    
    public const string GetCashFlow = """
                                          SELECT 
                                              "TransactionType", 
                                              SUM("Amount") as Total
                                          FROM "Transaction"
                                          WHERE "UserId" = @UserId
                                            AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                            AND "IsDeleted" = false
                                          GROUP BY "TransactionType";
                                      """;

    public const string ExistsByFixedExpense = """
                                                   SELECT COUNT(1) 
                                                   FROM "Transaction" 
                                                   WHERE "FixedExpenseId" = @FixedExpenseId
                                                     AND EXTRACT(MONTH FROM "TransactionDate") = @Month
                                                     AND EXTRACT(YEAR FROM "TransactionDate") = @Year
                                                     AND "IsDeleted" = false;
                                               """;
    
    public const string GetFamilyTransactionsPaged = """
                                                         WITH base AS (
                                                             SELECT t.*, COUNT(*) OVER() AS total_rows
                                                             FROM "Transaction" t
                                                             LEFT JOIN "User" u ON t."UserId" = u."Id"
                                                             WHERE (t."UserId" = @UserId OR u."ParentUserId" = @UserId)
                                                               AND t."TransactionDate" BETWEEN @StartDate AND @EndDate
                                                               AND (@TransactionType IS NULL OR t."TransactionType" = @TransactionType)
                                                               AND t."IsDeleted" = false
                                                         ),
                                                         paged AS (
                                                             SELECT * FROM base
                                                             ORDER BY
                                                                 "TransactionDate" DESC
                                                             LIMIT @RowsPerPage OFFSET (@CurrentPage - 1) * @RowsPerPage
                                                         )
                                                         SELECT *, total_rows,
                                                                CEILING(total_rows::decimal / @RowsPerPage) AS total_pages,
                                                                @CurrentPage AS current_page, @RowsPerPage AS rows_per_page
                                                         FROM paged;
                                                     """;
    
}