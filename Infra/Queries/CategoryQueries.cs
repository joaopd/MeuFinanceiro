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
                                 """;
    
    public const string GetAll = """
                                     SELECT "Id", "Name", "CreatedAt" 
                                     FROM "Category" 
                                     WHERE "IsDeleted" = false;
                                 """;
    
    public const string GetById = """
                                       SELECT "Id", "Name", "CreatedAt" 
                                       FROM "Category" 
                                       WHERE "Id" = @Id 
                                         AND "IsDeleted" = false;
                                   """;
}