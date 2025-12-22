namespace Infra.Queries;

public static class CardQueries
{
    public const string Create = @"
        INSERT INTO ""Card"" (""Id"", ""Name"", ""CreditLimit"", ""UserId"", ""ClosingDay"", ""DueDay"", ""Color"", ""CreatedAt"", ""IsDeleted"")
        VALUES (@Id, @Name, @CreditLimit, @UserId, @ClosingDay, @DueDay, @Color, @CreatedAt, @IsDeleted);
    ";

    public const string Update = @"
        UPDATE ""Card""
        SET ""Name"" = @Name,
            ""CreditLimit"" = @CreditLimit,
            ""ClosingDay"" = @ClosingDay,
            ""DueDay"" = @DueDay,
            ""Color"" = @Color, 
            ""UpdatedAt"" = @UpdatedAt
        WHERE ""Id"" = @Id;
    ";

    public const string Delete = @"
        UPDATE ""Card""
        SET ""IsDeleted"" = true,
            ""UpdatedAt"" = @UpdatedAt
        WHERE ""Id"" = @Id;
    ";

    public const string GetById = @"SELECT * FROM ""Card"" WHERE ""Id"" = @Id AND ""IsDeleted"" = false;";

    public const string GetByUserId = @"SELECT * FROM ""Card"" WHERE ""UserId"" = @UserId AND ""IsDeleted"" = false ORDER BY ""Name"";";

    public const string GetFamilyCards = @"
        SELECT c.* FROM ""Card"" c
        LEFT JOIN ""User"" u ON c.""UserId"" = u.""Id""
        WHERE (c.""UserId"" = @UserId OR u.""ParentUserId"" = @UserId)
          AND c.""IsDeleted"" = false
        ORDER BY c.""Name"";
    ";
}