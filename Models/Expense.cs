namespace expense_splitter_blazor.Models;

/// <summary>
/// How an expense's total amount is divided among its participants.
/// </summary>
public enum SplitType
{
    /// <summary>Divided evenly among the selected participants.</summary>
    Equal,

    /// <summary>Each participant's share is entered explicitly and must add up to the total.</summary>
    Custom
}

/// <summary>
/// A single purchase logged against a group: who paid, how much, and how it is split
/// (see <see cref="Splits"/> for each participant's individual share).
/// </summary>
public class Expense
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public Group? Group { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DateUtc { get; set; } = DateTime.UtcNow;

    public int PaidByPersonId { get; set; }

    public Person? PaidByPerson { get; set; }

    public SplitType SplitType { get; set; } = SplitType.Equal;

    public List<ExpenseSplit> Splits { get; set; } = new();
}
