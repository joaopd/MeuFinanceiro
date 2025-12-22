using FluentMigrator;

namespace Infra.Migrations;

[Migration(202512221230)]
public class AddInvoiceSupport : Migration
{
    public override void Up()
    {
        Create.Table("Invoices")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("CardId").AsGuid().NotNullable()
            .WithColumn("ReferenceDate").AsDateTime().NotNullable()
            .WithColumn("DueDate").AsDateTime().NotNullable()
            .WithColumn("TotalAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("IsPaid").AsBoolean().NotNullable().WithDefaultValue(false)

            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsGuid().Nullable()
            .WithColumn("UpdatedAt").AsDateTime().Nullable()
            .WithColumn("UpdatedBy").AsGuid().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);

        // FK Invoice -> Card
        Create.ForeignKey("FK_Invoices_Cards")
            .FromTable("Invoices").ForeignColumn("CardId")
            .ToTable("Cards").PrimaryColumn("Id");

        // Índice importante (card + referência mensal)
        Create.Index("IX_Invoices_Card_ReferenceDate")
            .OnTable("Invoices")
            .OnColumn("CardId").Ascending()
            .OnColumn("ReferenceDate").Ascending();

        Alter.Table("Transaction")
            .AddColumn("InvoiceId")
            .AsGuid()
            .Nullable();

        Create.ForeignKey("FK_Transaction_Invoice")
            .FromTable("Transaction").ForeignColumn("InvoiceId")
            .ToTable("Invoices").PrimaryColumn("Id");

        Create.Index("IX_Transaction_InvoiceId")
            .OnTable("Transaction")
            .OnColumn("InvoiceId");
    }

    public override void Down()
    {
        Delete.Index("IX_Transaction_InvoiceId").OnTable("Transaction");
        Delete.ForeignKey("FK_Transaction_Invoice").OnTable("Transaction");
        Delete.Column("InvoiceId").FromTable("Transaction");

        Delete.Index("IX_Invoices_Card_ReferenceDate").OnTable("Invoices");
        Delete.ForeignKey("FK_Invoices_Cards").OnTable("Invoices");
        Delete.Table("Invoices");
    }
}
