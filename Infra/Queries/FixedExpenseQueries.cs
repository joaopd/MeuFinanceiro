namespace Infra.Queries;

public static class FixedExpenseQueries
{
    public const string GetById = """
                                      SELECT * FROM "FixedExpense"
                                      WHERE "Id" = @Id AND "IsDeleted" = false;
                                  """;

    public const string GetAll = """
                                     SELECT * FROM "FixedExpense"
                                     WHERE "IsDeleted" = false;
                                 """;

    public const string GetActiveByUser = @"
                                    SELECT 
                                        ""Id"", 
                                        ""UserId"", 
                                        ""Description"", 
                                        ""Amount"", 
                                        ""StartDate""::timestamp AS ""StartDate"", 
                                        ""EndDate""::timestamp AS ""EndDate"", 
                                        ""Recurrence"", 
                                        ""CategoryId"", 
                                        ""CardId"", 
                                        ""IsDeleted""
                                    FROM ""FixedExpense""
                                    WHERE ""UserId"" = @UserId
                                      AND ""StartDate"" <= @ReferenceDate
                                      AND (""EndDate"" IS NULL OR ""EndDate"" >= @ReferenceDate)
                                      AND ""IsDeleted"" = false;";

    public const string Insert = """
                                     INSERT INTO "FixedExpense" (
                                         "Id",
                                         "UserId",
                                         "Description",
                                         "Amount",
                                         "StartDate",
                                         "EndDate",
                                         "Recurrence",
                                         "CategoryId",
                                         "CardId",
                                         "CreatedAt",
                                         "UpdatedAt",
                                         "IsDeleted"
                                     )
                                     VALUES (
                                         @Id,
                                         @UserId,
                                         @Description,
                                         @Amount,
                                         @StartDate,
                                         @EndDate,
                                         @Recurrence,
                                         @CategoryId,
                                         @CardId,
                                         @CreatedAt,
                                         @UpdatedAt,
                                         false
                                     )
                                     RETURNING "Id";
                                 """;

    public const string Update = """
                                     UPDATE "FixedExpense"
                                     SET
                                         "Description" = @Description,
                                         "Amount" = @Amount,
                                         "EndDate" = @EndDate,
                                         "UpdatedAt" = @UpdatedAt,
                                         "UpdatedBy" = @UpdatedBy
                                     WHERE "Id" = @Id;
                                 """;

    public const string SoftDelete = """
                                         UPDATE "FixedExpense"
                                         SET "IsDeleted" = true, "UpdatedAt" = @UpdatedAt, "UpdatedBy" = @UpdatedBy
                                         WHERE "Id" = @Id;
                                     """;
}