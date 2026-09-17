# Taxi Fleet Manager

An ASP.NET Core MVC web app for managing a small minibus taxi fleet: vehicles, drivers, routes, trips, and expenses, with a separate PIN-based login flow for drivers to log their own trips.

> **Status: work in progress.** Several features described below are stubs or not yet built. See [Known Limitations](#known-limitations--todo) before assuming anything is finished.

## Tech Stack

- **Framework:** ASP.NET Core MVC, .NET 10
- **ORM:** Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Database:** SQL Server (LocalDB by default)
- **Frontend:** Razor views, Bootstrap 5, jQuery + jQuery Validation
- **Auth:** No ASP.NET Identity — driver access is a custom session-based PIN gate (see below)

## Features

### Owner / admin side
- Full CRUD for **Vehicles** (registration, make, model, capacity, status, assigned driver)
- Full CRUD for **Drivers** (contact info, license number, hire date, status, PIN)
- Full CRUD for **Taxi Routes** (start/end points, standard fare, route type, active flag)
- Full CRUD for **Expenses**, linked to a vehicle
- **Trips**: view, edit, and delete (no admin-side "create" screen — trips are created by drivers, see below)
- **Owner Profile / Settings**: single editable record (business name, contact, banking details) — no create/delete, since there's only ever one

### Driver side
- `DriverLogin` — driver picks their name and enters a 4-digit PIN
- `DriverDashboard` — landing page after login
- `LogTrip` — driver logs a completed trip (vehicle, route, passengers, amount collected, payment method/reference)
- `DriverEdit` — driver can edit a trip they logged
- `DriverLogout` — clears the session

## Data Model

| Entity | Key fields | Relationships |
|---|---|---|
| `Vehicle` | Registration, Make, Model, Capacity, Status | has one `Driver` (optional), many `Trips`, many `Expenses` |
| `Driver` | FullName, ContactNumber, LicenseNumber, HireDate, Status, Pin | many `Trips`, many `Vehicles` |
| `TaxiRoute` | Name, StartPoint, EndPoint, StandardFare, RouteType, IsActive | many `Trips` |
| `Trip` | TripDate, PassengerCount, AmountCollected, PaymentMethod, PaymentReference | belongs to `Vehicle`, `Driver`, `TaxiRoute` |
| `Expense` | ExpenseType, Amount, ExpenseDate, Description | belongs to `Vehicle` |
| `OwnerProfile` | BusinessName, FullName, ContactNumber, BankName, AccountNumber, BranchCode | standalone |

## Getting Started (Visual Studio)

1. **Open the project.** Launch Visual Studio and open `TaxiFleetManager.csproj` (or the `.slnx` solution file if one is included in your checkout).
2. **Confirm workloads.** Make sure the **ASP.NET and web development** workload is installed (Visual Studio Installer → Modify, if needed), since this is a .NET 10 web project.
3. **Restore NuGet packages.** Visual Studio does this automatically on open; if not, right-click the solution in Solution Explorer → **Restore NuGet Packages**.
4. **Set up the database.** The connection string in `appsettings.json` points to LocalDB:
   ```
   Server=(localdb)\MSSQLLocalDB;Database=TaxiFleetManagerDB;Trusted_Connection=True;TrustServerCertificate=True;
   ```
   This project currently has **no `Migrations` folder** — the `ApplicationDbContext` was scaffolded from an existing database rather than built code-first. That means running `dotnet ef database update` will not create the schema for you. Until migrations are added, you'll need to either:
   - Point the connection string at a SQL Server instance that already has the `TaxiFleetManagerDB` schema (matching the tables implied by the `Models/` classes), or
   - Generate an initial migration yourself once the model is finalized: open **Tools → NuGet Package Manager → Package Manager Console** and run
     ```
     Add-Migration InitialCreate
     Update-Database
     ```
5. **Run the app.** Press **F5** (or **Ctrl+F5** to run without debugging) with `TaxiFleetManager` set as the startup project.
6. **Navigate.** The home page (`Views/Home/Index.cshtml`) offers two entry points: **"I'm the Owner"** (goes to Vehicles) and **"I'm a Driver"** (goes to `Trips/DriverLogin`).

## Project Structure

```
Controllers/     - MVC controllers (one per entity, plus Trips for the driver flow)
Models/          - EF Core entity classes
Data/            - ApplicationDbContext
Views/           - Razor views, organised by controller
Program.cs       - App startup, DI, middleware pipeline
appsettings.json - Connection string and logging config
```

## Known Limitations / TODO

This app is **not complete**. Known gaps as of this README:

- No database migrations checked in — schema must be created/matched manually (see step 4 above).
- No admin-side "Create Trip" view — trips can only be created via the driver `LogTrip` flow. If an owner needs to add a trip manually, this is missing.
- No `Create`/`Delete` actions for `OwnerProfile` — this assumes exactly one owner profile row already exists in the database.
- Driver PIN authentication is a simple 4-digit code compared server-side against a session — it is **not hashed or salted**, and there's no lockout/rate-limiting on failed attempts. Do not treat this as production-grade auth.
- No authentication/authorization on the owner/admin side at all — any visitor can reach `/Vehicles`, `/Drivers`, etc. with no login.
- No automated tests.
- No validation review yet on decimal/currency fields (e.g. fare, amounts) for negative values or bounds.

---
