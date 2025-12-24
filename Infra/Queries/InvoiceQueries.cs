namespace Infra.Queries;

public static class InvoiceQueries
{
    // AJUSTE AQUI: Mudamos de ::date para ::timestamp
    // Isso garante que o retorno seja compatível com DateTime no C#
    private const string SelectColumns = """
        SELECT 
            i."Id", 
            i."CardId", 
            i."ReferenceDate"::timestamp, 
            i."DueDate"::timestamp, 
            i."TotalAmount", 
            i."IsPaid", 
            i."CreatedAt", 
            i."UpdatedAt", 
            i."CreatedBy", 
            i."UpdatedBy", 
            i."IsDeleted"
        FROM "Invoice" i
    """;

    public const string GetByCardAndDate = $"""
        {SelectColumns}
        WHERE i."CardId" = @CardId
          AND EXTRACT(MONTH FROM i."ReferenceDate") = @Month
          AND EXTRACT(YEAR FROM i."ReferenceDate") = @Year
          AND i."IsDeleted" = false;
    """;

    public const string GetByUserId = $"""
        {SelectColumns}
        INNER JOIN "Card" c ON i."CardId" = c."Id"
        WHERE c."UserId" = @UserId
          AND i."IsDeleted" = false
        ORDER BY i."DueDate" DESC;
    """;

    // O Insert permanece igual, pois o Postgres aceita DateTime para colunas do tipo date
    public const string Insert = """
        INSERT INTO "Invoice" (
            "Id", "CardId", "ReferenceDate", "DueDate",
            "TotalAmount", "IsPaid", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted"
        )
        VALUES (
            @Id, @CardId, @ReferenceDate, @DueDate,
            @TotalAmount, @IsPaid, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy, @IsDeleted
        );
    """;

    public const string GetById = $"""
        {SelectColumns}
        WHERE i."Id" = @Id
          AND i."IsDeleted" = false;
    """;

    public const string PayInvoice = """
        UPDATE "Invoice"
        SET "IsPaid" = true,
            "UpdatedAt" = CURRENT_TIMESTAMP,
            "UpdatedBy" = @UserId
        WHERE "Id" = @InvoiceId
          AND "IsDeleted" = false;
    """;

    public const string PayRelatedTransactions = """
        UPDATE "Transaction"
        SET "IsPaid" = true,
            "UpdatedAt" = CURRENT_TIMESTAMP,
            "UpdatedBy" = @UserId
        WHERE "InvoiceId" = @InvoiceId
          AND "IsDeleted" = false;
    """;

    public const string GetAll = $"""
        {SelectColumns}
        WHERE i."IsDeleted" = false
        ORDER BY i."DueDate" DESC;
    """;

    public const string Update = """
        UPDATE "Invoice" SET
            "ReferenceDate" = @ReferenceDate,
            "DueDate" = @DueDate,
            "TotalAmount" = @TotalAmount,
            "IsPaid" = @IsPaid,
            "UpdatedAt" = @UpdatedAt,
            "UpdatedBy" = @UpdatedBy
        WHERE "Id" = @Id
          AND "IsDeleted" = false;
    """;

    public const string Delete = """
        UPDATE "Invoice"
        SET "IsDeleted" = true,
            "UpdatedAt" = CURRENT_TIMESTAMP,
            "UpdatedBy" = @UserId
        WHERE "Id" = @Id;
    """;
}