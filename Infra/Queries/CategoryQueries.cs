namespace Infra.Queries;

public static class CategoryQueries
{
    public const string Insert = """
                                     INSERT INTO "Category" (
                                         "Id",
                                         "Name",
                                         "CreatedAt",
                                         "UpdatedAt",
                                         "IsDeleted"
                                     )
                                     VALUES (
                                         @Id,
                                         @Name,
                                         @CreatedAt,
                                         @UpdatedAt,
                                         false
                                     )
                                     RETURNING "Id";
                                 """;

    public const string GetAll = """
                                     SELECT *
                                     FROM "Category"
                                     WHERE "IsDeleted" = false
                                     ORDER BY "Name";
                                 """;

    public const string GetById = """
                                      SELECT *
                                      FROM "Category"
                                      WHERE "Id" = @Id
                                        AND "IsDeleted" = false;
                                  """;

    public const string GetByName = """
                                        SELECT *
                                        FROM "Category"
                                        WHERE "Name" ILIKE @Name
                                          AND "IsDeleted" = false
                                        LIMIT 1;
                                    """;

    public const string Update = """
                                     UPDATE "Category"
                                     SET
                                         "Name" = @Name,
                                         "UpdatedAt" = @UpdatedAt,
                                         "UpdatedBy" = @UpdatedBy
                                     WHERE "Id" = @Id;
                                 """;

    public const string SoftDelete = """
                                         UPDATE "Category"
                                         SET
                                             "IsDeleted" = true,
                                             "UpdatedAt" = @UpdatedAt
                                         WHERE "Id" = @Id;
                                     """;
}