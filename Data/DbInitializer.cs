using expense_splitter_blazor.Models;

namespace expense_splitter_blazor.Data;

/// <summary>
/// Seeds a sample group on first run so the app is immediately demonstrable
/// without manual data entry. No-ops if any group already exists.
/// </summary>
public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Groups.Any())
        {
            return;
        }

        var group = new Group { Name = "Trip to Lahore" };

        var ali = new Person { Name = "Ali" };
        var sara = new Person { Name = "Sara" };
        var bilal = new Person { Name = "Bilal" };

        group.Members.Add(ali);
        group.Members.Add(sara);
        group.Members.Add(bilal);

        db.Groups.Add(group);
        db.SaveChanges(); // persist so members get IDs to reference below

        var hotel = new Expense
        {
            GroupId = group.Id,
            Description = "Hotel (2 nights)",
            Amount = 900m,
            PaidByPersonId = ali.Id,
            SplitType = SplitType.Equal,
            DateUtc = DateTime.UtcNow.AddDays(-2)
        };
        hotel.Splits.Add(new ExpenseSplit { PersonId = ali.Id, Amount = 300m });
        hotel.Splits.Add(new ExpenseSplit { PersonId = sara.Id, Amount = 300m });
        hotel.Splits.Add(new ExpenseSplit { PersonId = bilal.Id, Amount = 300m });

        var dinner = new Expense
        {
            GroupId = group.Id,
            Description = "Dinner at Cuckoo's Den",
            Amount = 450m,
            PaidByPersonId = sara.Id,
            SplitType = SplitType.Equal,
            DateUtc = DateTime.UtcNow.AddDays(-1)
        };
        dinner.Splits.Add(new ExpenseSplit { PersonId = ali.Id, Amount = 150m });
        dinner.Splits.Add(new ExpenseSplit { PersonId = sara.Id, Amount = 150m });
        dinner.Splits.Add(new ExpenseSplit { PersonId = bilal.Id, Amount = 150m });

        var cab = new Expense
        {
            GroupId = group.Id,
            Description = "Cab to Walled City",
            Amount = 200m,
            PaidByPersonId = bilal.Id,
            SplitType = SplitType.Custom,
            DateUtc = DateTime.UtcNow
        };
        cab.Splits.Add(new ExpenseSplit { PersonId = ali.Id, Amount = 100m });
        cab.Splits.Add(new ExpenseSplit { PersonId = sara.Id, Amount = 60m });
        cab.Splits.Add(new ExpenseSplit { PersonId = bilal.Id, Amount = 40m });

        db.Expenses.AddRange(hotel, dinner, cab);
        db.SaveChanges();
    }
}
