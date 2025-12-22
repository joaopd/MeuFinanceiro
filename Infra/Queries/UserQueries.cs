namespace Infra.Queries;

public static class UserQueries
{
    public const string Insert = """
                                     INSERT INTO "User"
                                     (
                                         "Id",
                                         "Name",
                                         "Email",
                                         "ParentUserId",
                                         "CreatedAt",
                                         "UpdatedAt",
                                         "IsDeleted"
                                     )
                                     VALUES
                                     (
                                         @Id,
                                         @Name,
                                         @Email,
                                         @ParentUserId,
                                         @CreatedAt,
                                         @UpdatedAt,
                                         @IsDeleted
                                     )
                                     RETURNING "Id";
                                 """;

    public const string GetById = """
                                      SELECT *
                                      FROM "User"
                                      WHERE "Id" = @Id
                                        AND "IsDeleted" = false;
                                  """;
    public const string GetByEmail = """
                                      SELECT *
                                      FROM "User"
                                      WHERE "Email" = @Email
                                        AND "IsDeleted" = false;
                                  """;

    public const string GetDependents = """
                                            SELECT *
                                            FROM "User"
                                            WHERE "ParentUserId" = @ParentUserId
                                              AND "IsDeleted" = false;
                                        """;
    
    public const string GetByParentId = """
                                            SELECT * FROM "User"
                                            WHERE "ParentUserId" = @ParentUserId
                                              AND "IsDeleted" = false;
                                        """;
}