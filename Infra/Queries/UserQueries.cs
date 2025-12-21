namespace Infra.Queries;

public static class UserQueries
{
    public const string Insert = """
                                     INSERT INTO "Users"
                                     (
                                         "Id",
                                         "Name",
                                         "ParentUserId",
                                         "CreatedAt",
                                         "UpdatedAt",
                                         "IsDeleted"
                                     )
                                     VALUES
                                     (
                                         @Id,
                                         @Name,
                                         @ParentUserId,
                                         @CreatedAt,
                                         @UpdatedAt,
                                         @IsDeleted
                                     )
                                     RETURNING "Id";
                                 """;

    public const string GetById = """
                                      SELECT *
                                      FROM "Users"
                                      WHERE "Id" = @Id
                                        AND "IsDeleted" = false;
                                  """;

    public const string GetDependents = """
                                            SELECT *
                                            FROM "Users"
                                            WHERE "ParentUserId" = @ParentUserId
                                              AND "IsDeleted" = false;
                                        """;
}