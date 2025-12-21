namespace Infra.Queries;

public static class FixedExpenseQueries
{
    public const string GetById = """
                                      SELECT *
                                      FROM "FixedExpenses"
                                      WHERE "Id" = @Id
                                        AND "IsDeleted" = false;
                                  """;

    public const string GetAll = """
                                     SELECT *
                                     FROM "FixedExpenses"
                                     WHERE "IsDeleted" = false;
                                 """;

    public const string GetActiveByUser = """
                                              SELECT *
                                              FROM "FixedExpenses"
                                              WHERE "UserId" = @UserId
                                                AND "StartDate" <= @ReferenceDate
                                                AND ("EndDate" IS NULL OR "EndDate" >= @ReferenceDate)
                                                AND "IsDeleted" = false;
                                          """;

    public const string Insert = """
                                     INSERT INTO "FixedExpenses"
                                     (
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
                                     VALUES
                                     (
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
                                         @IsDeleted
                                     )
                                     RETURNING "Id";
                                 """;

    public const string Update = """
                                     UPDATE "FixedExpenses"
                                     SET
                                         "Description" = @Description,
                                         "Amount" = @Amount,
                                         "EndDate" = @EndDate,
                                         "UpdatedAt" = @UpdatedAt
                                     WHERE "Id" = @Id;
                                 """;

    public const string SoftDelete = """
                                         UPDATE "FixedExpenses"
                                         SET
                                             "IsDeleted" = true,
                                             "UpdatedAt" = @UpdatedAt
                                         WHERE "Id" = @Id;
                                     """;
}