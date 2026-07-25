using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string RegistrationNumber { get; set; } = null!;

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public int? DriverId { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
