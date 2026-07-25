using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public int VehicleId { get; set; }

    public string ExpenseType { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string? Description { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
