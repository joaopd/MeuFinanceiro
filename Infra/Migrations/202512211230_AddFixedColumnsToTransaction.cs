using FluentMigrator;

namespace Infra.Migrations;

[Migration(202512211230)]
public class AddFixedColumnsToTransaction : Migration
{
    public override void Up()
    {
        Alter.Table("Transaction")
            .AddColumn("IsFixed")
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false)
            .AddColumn("FixedExpenseId")
            .AsGuid()
            .Nullable()
            .ForeignKey("FK_Transaction_FixedExpense", "FixedExpense", "Id");
    }

    public override void Down()
    {
        Delete.Column("FixedExpenseId").FromTable("Transaction");
        Delete.Column("IsFixed").FromTable("Transaction");
    }
}