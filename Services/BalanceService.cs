using expense_splitter_blazor.Models;

namespace expense_splitter_blazor.Services;

/// <summary>Net amount a person owes (negative) or is owed (positive) within a group.</summary>
public record PersonBalance(int PersonId, string PersonName, decimal NetAmount);

/// <summary>A single suggested settlement payment: one person pays another a fixed amount.</summary>
public record SettlementTransaction(int FromPersonId, string FromPersonName, int ToPersonId, string ToPersonName, decimal Amount);

/// <summary>
/// Computes who owes whom within a group and reduces that down to a minimal set of
/// point-to-point settlement payments. Kept independent of EF Core and Blazor so the
/// algorithm can be unit tested in isolation from persistence and rendering.
/// </summary>
public class BalanceService
{
    private const decimal Tolerance = 0.01m;

    /// <summary>
    /// Calculates each member's net position: total they paid across all expenses
    /// minus the total of their own splits across all expenses. Positive means the
    /// group owes them money; negative means they owe the group money.
    /// </summary>
    public IReadOnlyList<PersonBalance> CalculateNetBalances(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var running = group.Members.ToDictionary(m => m.Id, _ => 0m);

        foreach (var expense in group.Expenses)
        {
            if (running.ContainsKey(expense.PaidByPersonId))
            {
                running[expense.PaidByPersonId] += expense.Amount;
            }

            foreach (var split in expense.Splits)
            {
                if (running.ContainsKey(split.PersonId))
                {
                    running[split.PersonId] -= split.Amount;
                }
            }
        }

        return group.Members
            .Select(m => new PersonBalance(m.Id, m.Name, Math.Round(running[m.Id], 2)))
            .OrderByDescending(b => b.NetAmount)
            .ToList();
    }

    /// <summary>
    /// Reduces a set of net balances to a small list of settlement payments instead of
    /// showing every pairwise debt that produced them (e.g. if A owes B and B owes C,
    /// this nets it down to "A pays C" rather than listing both original debts).
    ///
    /// Uses a greedy match: repeatedly pay the largest remaining creditor from the
    /// largest remaining debtor. This does not always produce the mathematically
    /// minimal number of transactions, but it always fully settles the group and
    /// never produces more transactions than the number of people involved.
    /// </summary>
    public IReadOnlyList<SettlementTransaction> SimplifyDebts(IEnumerable<PersonBalance> balances)
    {
        ArgumentNullException.ThrowIfNull(balances);

        var creditors = balances
            .Where(b => b.NetAmount > Tolerance)
            .Select(b => (b.PersonId, b.PersonName, Amount: b.NetAmount))
            .ToList();

        var debtors = balances
            .Where(b => b.NetAmount < -Tolerance)
            .Select(b => (b.PersonId, b.PersonName, Amount: -b.NetAmount)) // store as a positive amount owed
            .ToList();

        var transactions = new List<SettlementTransaction>();

        while (creditors.Count > 0 && debtors.Count > 0)
        {
            creditors.Sort((a, b) => b.Amount.CompareTo(a.Amount));
            debtors.Sort((a, b) => b.Amount.CompareTo(a.Amount));

            var creditor = creditors[0];
            var debtor = debtors[0];

            var settleAmount = Math.Round(Math.Min(creditor.Amount, debtor.Amount), 2);
            if (settleAmount <= 0)
            {
                break;
            }

            transactions.Add(new SettlementTransaction(
                debtor.PersonId, debtor.PersonName,
                creditor.PersonId, creditor.PersonName,
                settleAmount));

            creditors[0] = (creditor.PersonId, creditor.PersonName, creditor.Amount - settleAmount);
            debtors[0] = (debtor.PersonId, debtor.PersonName, debtor.Amount - settleAmount);

            creditors.RemoveAll(c => c.Amount <= Tolerance);
            debtors.RemoveAll(d => d.Amount <= Tolerance);
        }

        return transactions;
    }
}
