using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Infra.Migrations;

namespace Infra.Database.Migrations;

[Migration(202512122351)]
public class FirstMigration : Migration
{
    // ============================
    // COLUNAS PADRÃO
    // ============================
    private const string Id = "Id";
    private const string CreatedAt = "CreatedAt";
    private const string UpdatedAt = "UpdatedAt";
    private const string CreatedBy = "CreatedBy";
    private const string UpdatedBy = "UpdatedBy";
    private const string IsDeleted = "IsDeleted";

    public override void Up()
    {
        CreateUser();
        CreateCard();
        CreateCategory();
        CreateFixedExpense();
        CreateTransaction();
    }

    public override void Down()
    {
        Delete.Table("Transaction");
        Delete.Table("FixedExpense");
        Delete.Table("Category");
        Delete.Table("Card");
        Delete.Table("User");
    }

    // =======================================
    // USER
    // =======================================
    private void CreateUser()
    {
        Create.Table("User")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(150).NotNullable()
            .WithColumn("Email").AsString(150).NotNullable()
            .WithColumn("ParentUserId").AsGuid().Nullable()

            .WithCommonColumns();

        Create.Index("IX_User_ParentUserId")
            .OnTable("User")
            .OnColumn("ParentUserId");
    }

    // =======================================
    // CARD
    // =======================================
    private void CreateCard()
    {
        Create.Table("Card")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("PaymentMethod").AsInt16().NotNullable()
            .WithColumn("CreditLimit").AsDecimal(18, 2).Nullable()

            .WithCommonColumns();
    }

    // =======================================
    // CATEGORY
    // =======================================
    private void CreateCategory()
    {
        Create.Table("Category")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(100).NotNullable()
            // Removido ExpenseType
            .WithCommonColumns();
    }

    // =======================================
    // FIXED EXPENSE
    // =======================================
    private void CreateFixedExpense()
    {
        Create.Table("FixedExpense")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid().NotNullable()
                .ForeignKey("FK_FixedExpense_User", "User", Id)
            .WithColumn("Description").AsString(200).NotNullable()
            .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
            .WithColumn("StartDate").AsDate().NotNullable()
            .WithColumn("EndDate").AsDate().Nullable()
            .WithColumn("Recurrence").AsInt16().NotNullable()
            .WithColumn("CategoryId").AsGuid().NotNullable()
                .ForeignKey("FK_FixedExpense_Category", "Category", Id)
            .WithColumn("CardId").AsGuid().Nullable()
                .ForeignKey("FK_FixedExpense_Card", "Card", Id)

            .WithCommonColumns();

        Create.Index("IX_FixedExpense_UserId")
            .OnTable("FixedExpense")
            .OnColumn("UserId");

        Create.Index("IX_FixedExpense_CategoryId")
            .OnTable("FixedExpense")
            .OnColumn("CategoryId");
    }

    // =======================================
    // TRANSACTION
    // =======================================
    private void CreateTransaction()
    {
        Create.Table("Transaction")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid().NotNullable()
            .ForeignKey("FK_Transaction_User", "User", Id)
            .WithColumn("CategoryId").AsGuid().NotNullable()
            .ForeignKey("FK_Transaction_Category", "Category", Id)
            .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
            .WithColumn("TransactionDate").AsDateTime().NotNullable()
            .WithColumn("TransactionType").AsInt16().NotNullable()
            .WithColumn("PaymentMethod").AsInt16().NotNullable()
            
            .WithColumn("CardId").AsGuid().Nullable()
            .ForeignKey("FK_Transaction_Card", "Card", Id) 

            .WithColumn("InstallmentNumber").AsInt32().Nullable()
            .WithColumn("TotalInstallments").AsInt32().Nullable()
            
            .WithColumn("IsPaid").AsBoolean().NotNullable().WithDefaultValue(false)
            
            .WithCommonColumns();
        
        Create.Index("IX_Transaction_UserId")
            .OnTable("Transaction")
            .OnColumn("UserId");

        Create.Index("IX_Transaction_Date")
            .OnTable("Transaction")
            .OnColumn("TransactionDate");
    }
}
