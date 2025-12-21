using FluentMigrator;

namespace Infra.Migrations;

[Migration(202512211200)]
public class AddObservationToTransaction : Migration
{
    public override void Up()
    {
        Alter.Table("Transaction")
            .AddColumn("Observation").AsString(500).Nullable();
    }

    public override void Down()
    {
        Delete.Column("Observation").FromTable("Transaction");
    }
}