namespace Infra.Queries;

public static class CardQueries
{
    // Listamos as colunas para garantir que o Dapper mapeie corretamente
    // e para evitar trazer dados desnecessários em joins futuros.
    private const string SelectColumns = """
        SELECT 
            c."Id", 
            c."Name", 
            c."CreditLimit", 
            c."UserId", 
            c."ClosingDay", 
            c."DueDay", 
            c."Color", 
            c."CreatedAt", 
            c."UpdatedAt", 
            c."CreatedBy", 
            c."UpdatedBy", 
            c."IsDeleted"
        FROM "Card" c
    """;

    public const string Insert = """
        INSERT INTO "Card" (
            "Id", "Name", "CreditLimit", 
            "UserId", "ClosingDay", "DueDay", "Color", 
            "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted"
        )
        VALUES (
            @Id, @Name, @CreditLimit, 
            @UserId, @ClosingDay, @DueDay, @Color, 
            @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, false
        );
    """;

    public const string Update = """
        UPDATE "Card"
        SET 
            "Name" = @Name,
            "CreditLimit" = @CreditLimit,
            "ClosingDay" = @ClosingDay,
            "DueDay" = @DueDay,
            "Color" = @Color, 
            "UpdatedAt" = @UpdatedAt,
            "UpdatedBy" = @UpdatedBy
        WHERE "Id" = @Id;
    """;

    public const string SoftDelete = """
        UPDATE "Card"
        SET "IsDeleted" = true,
            "UpdatedAt" = @UpdatedAt,
            "UpdatedBy" = @UpdatedBy
        WHERE "Id" = @Id;
    """;

    public const string GetById = $"""
        {SelectColumns}
        WHERE c."Id" = @Id 
          AND c."IsDeleted" = false;
    """;

    public const string GetByUserId = $"""
        {SelectColumns}
        WHERE c."UserId" = @UserId 
          AND c."IsDeleted" = false 
        ORDER BY c."Name";
    """;

    public const string GetFamilyCards = $"""
        {SelectColumns}
        LEFT JOIN "User" u ON c."UserId" = u."Id"
        WHERE (c."UserId" = @UserId OR u."ParentUserId" = @UserId)
          AND c."IsDeleted" = false
        ORDER BY c."Name";
    """;
}