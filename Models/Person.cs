namespace expense_splitter_blazor.Models;

/// <summary>
/// A member of a <see cref="Group"/>. People are scoped to a single group in this
/// app (matching a simple Splitwise-style "trip" model rather than a global contacts list).
/// </summary>
public class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int GroupId { get; set; }

    public Group? Group { get; set; }

    public List<Expense> ExpensesPaid { get; set; } = new();

    public List<ExpenseSplit> Splits { get; set; } = new();
}
