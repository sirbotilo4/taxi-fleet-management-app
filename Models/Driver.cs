using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string FullName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Pin { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
