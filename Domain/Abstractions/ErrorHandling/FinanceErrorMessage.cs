namespace Domain.Abstractions.ErrorHandling;

public static class FinanceErrorMessage
{
    // ============================
    // GENERIC / INFRA
    // ============================
    public static readonly FinanceError InternalServerError =
        new("internal.server.error",
            "Internal server error.",
            ErrorType.InternalServerError);

    public static readonly FinanceError DatabaseError =
        new("database.error",
            "An error occurred while accessing the database.",
            ErrorType.InternalServerError);

    public static readonly FinanceError UnexpectedError =
        new("unexpected.error",
            "An unexpected error occurred.",
            ErrorType.InternalServerError);

    // ============================
    // USER
    // ============================
    public static readonly FinanceError UserNotFound =
        new("user.not.found",
            "User not found.",
            ErrorType.NotFound);

    public static readonly FinanceError UserAlreadyExists =
        new("user.already.exists",
            "User already exists.",
            ErrorType.Conflict);

    public static readonly FinanceError InvalidUserData =
        new("user.invalid.data",
            "Invalid user data.",
            ErrorType.Validation);

    public static readonly FinanceError CannotCreateDependentWithoutParent =
        new("user.dependent.parent.required",
            "A dependent user must have a parent user.",
            ErrorType.Validation);

    // ============================
    // CARD
    // ============================
    public static readonly FinanceError CardNotFound =
        new("card.not.found",
            "Card not found.",
            ErrorType.NotFound);

    public static readonly FinanceError CardAlreadyExists =
        new("card.already.exists",
            "Card already exists.",
            ErrorType.Conflict);

    public static readonly FinanceError InvalidCardData =
        new("card.invalid.data",
            "Invalid card data.",
            ErrorType.Validation);

    public static readonly FinanceError CreditLimitRequiredForCreditCard =
        new("card.credit.limit.required",
            "Credit limit is required for credit cards.",
            ErrorType.Validation);

    // ============================
    // CATEGORY
    // ============================
    public static readonly FinanceError CategoryNotFound =
        new("category.not.found",
            "Category not found.",
            ErrorType.NotFound);

    public static readonly FinanceError CategoryAlreadyExists =
        new("category.already.exists",
            "Category already exists.",
            ErrorType.Conflict);

    public static readonly FinanceError InvalidCategoryData =
        new("category.invalid.data",
            "Invalid category data.",
            ErrorType.Validation);

    // ============================
    // TRANSACTION
    // ============================
    public static readonly FinanceError TransactionNotFound =
        new("transaction.not.found",
            "Transaction not found.",
            ErrorType.NotFound);

    public static readonly FinanceError InvalidTransactionData =
        new("transaction.invalid.data",
            "Invalid transaction data.",
            ErrorType.Validation);

    public static readonly FinanceError TransactionAmountMustBeGreaterThanZero =
        new("transaction.amount.invalid",
            "Transaction amount must be greater than zero.",
            ErrorType.Validation);

    public static readonly FinanceError CardRequiredForCreditTransaction =
        new("transaction.card.required",
            "Card is required for credit transactions.",
            ErrorType.Validation);

    public static readonly FinanceError TransactionDateInFuture =
        new("transaction.date.future",
            "Transaction date cannot be in the future.",
            ErrorType.Validation);
    
    public static readonly FinanceError InvalidTransactionAmount =
        new("transaction.invalid.amount", "Transaction amount must be greater than zero.", ErrorType.Validation);

    public static readonly FinanceError PaymentMethodRequiredForCardTransaction =
        new("transaction.payment.method.required", "Payment method is required when using a card.", ErrorType.Validation);

    public static readonly FinanceError InvalidTransactionDate =
        new("transaction.invalid.date", "Transaction date is invalid.", ErrorType.Validation);
    
    public static readonly FinanceError InvalidPeriod =
        new("transaction.invalid.period.range", "The start date must be earlier than or equal to the end date.", ErrorType.Validation);

    // ============================
    // FIXED EXPENSE
    // ============================
    public static readonly FinanceError FixedExpenseNotFound =
        new("fixed.expense.not.found",
            "Fixed expense not found.",
            ErrorType.NotFound);

    public static readonly FinanceError InvalidFixedExpenseData =
        new("fixed.expense.invalid.data",
            "Invalid fixed expense data.",
            ErrorType.Validation);

    public static readonly FinanceError FixedExpenseStartDateInvalid =
        new("fixed.expense.start.date.invalid",
            "Start date must be less than or equal to end date.",
            ErrorType.Validation);

    public static readonly FinanceError FixedExpenseAlreadyEnded =
        new("fixed.expense.already.ended",
            "This fixed expense has already ended.",
            ErrorType.Conflict);

    // ============================
    // UPDATE / DELETE (GENERIC)
    // ============================
    public static readonly FinanceError EntityAlreadyDeleted =
        new("entity.already.deleted",
            "Entity is already deleted.",
            ErrorType.Conflict);

    public static readonly FinanceError UpdateNotAllowed =
        new("entity.update.not.allowed",
            "Update operation is not allowed.",
            ErrorType.Conflict);

    public static readonly FinanceError DeleteNotAllowed =
        new("entity.delete.not.allowed",
            "Delete operation is not allowed.",
            ErrorType.Conflict);
}
