namespace Infra.Queries;

public static class TransactionQueries
{
    public const string GetById = """
                                      SELECT *
                                      FROM "Transactions"
                                      WHERE "Id" = @Id
                                        AND "IsDeleted" = false;
                                  """;

    public const string GetAll = """
                                     SELECT *
                                     FROM "Transactions"
                                     WHERE "IsDeleted" = false;
                                 """;

    public const string GetByUserAndPeriod = """
                                                 SELECT *
                                                 FROM "Transactions"
                                                 WHERE "UserId" = @UserId
                                                   AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                                   AND "IsDeleted" = false;
                                             """;

    public const string Insert = """
                                   INSERT INTO "Transaction" (
                                     "Id",
                                     "UserId",
                                     "CategoryId",
                                     "Amount",
                                     "TransactionDate",
                                     "TransactionType",
                                     "CardId",
                                     "PaymentMethod",
                                     "InstallmentNumber",
                                     "TotalInstallments",
                                     "IsFixed",
                                     "IsPaid",
                                     "CreatedAt",
                                     "UpdatedAt",
                                     "IsDeleted"
                                 )
                                 VALUES (
                                     @Id,
                                     @UserId,
                                     @CategoryId,
                                     @Amount,
                                     @TransactionDate,
                                     @TransactionType,
                                     @CardId,
                                     @PaymentMethod,
                                     @InstallmentNumber,
                                     @TotalInstallments,
                                     @IsFixed,
                                     @IsPaid,
                                     @CreatedAt,
                                     @UpdatedAt,
                                     false
                                 )
                                 """;

    public const string SoftDelete = """
                                         UPDATE "Transactions"
                                         SET
                                             "IsDeleted" = true,
                                             "UpdatedAt" = @UpdatedAt
                                         WHERE "Id" = @Id;
                                     """;

    public const string GetBalanceByPeriod = """
                                                 SELECT
                                                     COALESCE(SUM(
                                                         CASE
                                                             WHEN "TransactionType" = 1 THEN "Amount" -- Income
                                                             WHEN "TransactionType" = 2 THEN - "Amount" -- Expense
                                                         END
                                                     ), 0)
                                                 FROM "Transactions"
                                                 WHERE "UserId" = @UserId
                                                   AND "TransactionDate" BETWEEN @StartDate AND @EndDate
                                                   AND "IsDeleted" = false;
                                             """;
    
    public const string GetByUserAndPeriodPagedWithMeta = """
                                                          WITH base AS (
                                                              SELECT
                                                                  t.*,
                                                                  COUNT(*) OVER() AS total_rows
                                                              FROM "Transaction" t
                                                              WHERE t."UserId" = @UserId
                                                                AND t."TransactionDate" BETWEEN @StartDate AND @EndDate
                                                                AND (@TransactionType IS NULL OR t."TransactionType" = @TransactionType)
                                                                AND t."IsDeleted" = false
                                                          ),
                                                          paged AS (
                                                              SELECT *
                                                              FROM base
                                                              ORDER BY
                                                                  CASE WHEN @OrderBy = 'TransactionDate' AND @OrderAsc THEN "TransactionDate" END ASC,
                                                                  CASE WHEN @OrderBy = 'TransactionDate' AND NOT @OrderAsc THEN "TransactionDate" END DESC,
                                                                  CASE WHEN @OrderBy = 'Amount' AND @OrderAsc THEN "Amount" END ASC,
                                                                  CASE WHEN @OrderBy = 'Amount' AND NOT @OrderAsc THEN "Amount" END DESC,
                                                                  "TransactionDate" DESC
                                                              LIMIT @RowsPerPage
                                                              OFFSET (@CurrentPage - 1) * @RowsPerPage
                                                          )
                                                          SELECT
                                                              *,
                                                              total_rows,
                                                              CEILING(total_rows::decimal / @RowsPerPage) AS total_pages,
                                                              @CurrentPage AS current_page,
                                                              @RowsPerPage AS rows_per_page
                                                          FROM paged;
                                                          """;

}