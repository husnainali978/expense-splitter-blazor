namespace expense_splitter_blazor.Models;

/// <summary>
/// A group of people who share expenses, e.g. a trip or a shared household.
/// </summary>
public class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<Person> Members { get; set; } = new();

    public List<Expense> Expenses { get; set; } = new();
}
