using FluentMigrator;
using Infra.Migrations;

namespace Infra.Database.Migrations;

[Migration(202512230001)]
public class FirstMigration : Migration
{
    private const string Id = "Id";

    public override void Up()
    {
        CreateUser();
        CreateCategory();
        CreateCard();
        CreateFixedExpense();
        CreateInvoice();
        CreateTransaction();
    }

    public override void Down()
    {
        Delete.Table("Transaction");
        Delete.Table("Invoice");
        Delete.Table("FixedExpense");
        Delete.Table("Card");
        Delete.Table("Category");
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
    // CATEGORY
    // =======================================
    private void CreateCategory()
    {
        Create.Table("Category")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithCommonColumns();
    }

    // =======================================
    // CARD
    // =======================================
    private void CreateCard()
    {
        Create.Table("Card")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("CreditLimit").AsDecimal(18, 2).Nullable()
            .WithColumn("UserId").AsGuid().NotNullable()
                .ForeignKey("FK_Card_User", "User", Id)
            .WithColumn("ClosingDay").AsInt32().NotNullable()
            .WithColumn("DueDay").AsInt32().NotNullable()
            .WithColumn("Color").AsString(20).Nullable()
            .WithCommonColumns();

        Create.Index("IX_Card_UserId")
            .OnTable("Card")
            .OnColumn("UserId");
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
    // INVOICE
    // =======================================
    private void CreateInvoice()
    {
        Create.Table("Invoice")
            .WithColumn(Id).AsGuid().PrimaryKey()
            .WithColumn("CardId").AsGuid().NotNullable()
                .ForeignKey("FK_Invoice_Card", "Card", Id)
            .WithColumn("ReferenceDate").AsDate().NotNullable()
            .WithColumn("DueDate").AsDate().NotNullable()
            .WithColumn("TotalAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("IsPaid").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCommonColumns();

        Create.Index("IX_Invoice_Card_ReferenceDate")
            .OnTable("Invoice")
            .OnColumn("CardId").Ascending()
            .OnColumn("ReferenceDate").Ascending();
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
            .WithColumn("PaymentMethod").AsInt16().Nullable()

            .WithColumn("CardId").AsGuid().Nullable()
                .ForeignKey("FK_Transaction_Card", "Card", Id)

            .WithColumn("InvoiceId").AsGuid().Nullable()
                .ForeignKey("FK_Transaction_Invoice", "Invoice", Id)

            .WithColumn("InstallmentNumber").AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn("TotalInstallments").AsInt32().NotNullable().WithDefaultValue(1)

            .WithColumn("IsFixed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("FixedExpenseId").AsGuid().Nullable()
                .ForeignKey("FK_Transaction_FixedExpense", "FixedExpense", Id)

            .WithColumn("IsPaid").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Observation").AsString(500).Nullable()

            .WithCommonColumns();

        Create.Index("IX_Transaction_UserId")
            .OnTable("Transaction")
            .OnColumn("UserId");

        Create.Index("IX_Transaction_Date")
            .OnTable("Transaction")
            .OnColumn("TransactionDate");

        Create.Index("IX_Transaction_InvoiceId")
            .OnTable("Transaction")
            .OnColumn("InvoiceId");
    }
}
