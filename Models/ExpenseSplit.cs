namespace expense_splitter_blazor.Models;

/// <summary>
/// One participant's share of an <see cref="Models.Expense"/>. The sum of an expense's
/// splits always equals the expense's total amount.
/// </summary>
public class ExpenseSplit
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public Expense? Expense { get; set; }

    public int PersonId { get; set; }

    public Person? Person { get; set; }

    /// <summary>The amount this person owes toward the expense.</summary>
    public decimal Amount { get; set; }
}
