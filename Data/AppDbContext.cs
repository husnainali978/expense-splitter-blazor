using expense_splitter_blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_splitter_blazor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<Person> People => Set<Person>();

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Members)
            .WithOne(p => p.Group!)
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Group>()
            .HasMany(g => g.Expenses)
            .WithOne(e => e.Group!)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.PaidByPerson)
            .WithMany(p => p.ExpensesPaid)
            .HasForeignKey(e => e.PaidByPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .HasMany(e => e.Splits)
            .WithOne(s => s.Expense!)
            .HasForeignKey(s => s.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExpenseSplit>()
            .HasOne(s => s.Person)
            .WithMany(p => p.Splits)
            .HasForeignKey(s => s.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ExpenseSplit>()
            .Property(s => s.Amount)
            .HasPrecision(18, 2);
    }
}
