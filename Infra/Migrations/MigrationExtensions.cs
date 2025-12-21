using FluentMigrator.Builders.Create.Table;

namespace Infra.Migrations;

public static class MigrationExtensions
{
    public static ICreateTableWithColumnSyntax WithCommonColumns(
        this ICreateTableWithColumnSyntax table)
    {
        return table
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsGuid().Nullable()
            .WithColumn("UpdatedBy").AsGuid().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
    }
}
