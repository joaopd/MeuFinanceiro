using FluentMigrator;

namespace Infra.Migrations;

[Migration(202512211900)]
public class AddColorToCard : Migration
{
    public override void Up()
    {
        Alter.Table("Card")
            .AddColumn("Color").AsString(7).NotNullable().WithDefaultValue("#000000"); // Hex Code
    }

    public override void Down()
    {
        Delete.Column("Color").FromTable("Card");
    }
}