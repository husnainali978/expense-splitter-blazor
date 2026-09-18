# Expense Splitter (Blazor Server)

A Splitwise-style expense splitter built entirely in C#: interactive UI, business
logic, and persistence all live in one ASP.NET Core process, with no separate
JavaScript frontend or API layer. It exists to demonstrate full-stack delivery in
.NET — Razor components driving server-rendered, stateful UI over a real-time
SignalR circuit — as a counterpoint to the more common React/Angular-plus-API-backend
split.

## Architecture

This is a standard **Blazor Server** app (interactive server-side rendering,
`net10.0`), not Blazor WebAssembly and not a separate SPA + Web API:

- **`Components/Pages/Groups/`** — the four routed pages:
  - `Index.razor` (`/`) — list of groups with member/expense counts and totals.
  - `Create.razor` (`/groups/create`) — create a group and its initial members.
  - `Detail.razor` (`/groups/{id}`) — members, expense history, and the
    add-expense form (equal or custom split).
  - `Balances.razor` (`/groups/{id}/balances`) — net balances and simplified
    settlement suggestions.
- **`Components/Layout/`** — `MainLayout.razor` / `NavMenu.razor`, the standard
  Blazor template shell (Bootstrap-based).
- **`Models/`** — plain EF Core entities: `Group`, `Person`, `Expense`,
  `ExpenseSplit`. An `Expense` has one payer and a collection of `ExpenseSplit`
  rows recording exactly what each participant owes toward it.
- **`Data/`** — `AppDbContext` (EF Core + SQLite) and `DbInitializer`, which
  seeds one sample group on first run so the app is immediately demonstrable.
- **`Services/BalanceService.cs`** — the debt-simplification logic, deliberately
  kept out of the Razor components:
  - `CalculateNetBalances(Group group)` nets each person's total paid against
    their total owed across every expense, producing one balance per person.
  - `SimplifyDebts(IEnumerable<PersonBalance> balances)` reduces those balances
    to a minimal set of point-to-point payments via a greedy
    largest-creditor/largest-debtor match, instead of listing every underlying
    pairwise debt (e.g. "A owes B" and "B owes C" collapse into "A pays C").

  Both methods are pure functions over plain models (no EF Core or Blazor
  dependency), so the algorithm can be unit tested or demoed in isolation from
  the UI and database.

Pages use `IDbContextFactory<AppDbContext>` (the recommended pattern for Blazor
Server) rather than an injected scoped `DbContext`, since a single circuit can
outlive a request-scoped lifetime.

## Features

- Create groups and add/remove members.
- Log expenses with a description, amount, payer, and participant list.
- Split an expense equally among selected participants, or enter custom
  per-person amounts (validated to sum to the total).
- Expense history per group, including each person's share of every expense.
- Net balance per group member (who is owed, who owes).
- Simplified settlement suggestions ("X pays Y $Z") instead of raw pairwise
  debts.
- SQLite persistence via EF Core; a sample "Trip to Lahore" group is seeded on
  first run.

## How to run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
cd expense-splitter-blazor
dotnet restore
dotnet run
```

Then open the URL printed in the console (e.g. `https://localhost:5001`). A
SQLite database file (`expensesplitter.db`) is created automatically on first
run, seeded with a sample group.

> If your environment has a broken/unreachable NuGet source configured
> globally, restore explicitly against nuget.org instead:
> `dotnet restore -s https://api.nuget.org/v3/index.json`.

## Tech stack

Blazor Server (.NET 10, C#, interactive server-side rendering) + EF Core 10 +
SQLite + Bootstrap.
