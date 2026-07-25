using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class Trip
{
    public int TripId { get; set; }

    public int VehicleId { get; set; }

    public int DriverId { get; set; }

    public int RouteId { get; set; }

    public DateTime TripDate { get; set; }

    public int PassengerCount { get; set; }

    public decimal AmountCollected { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentReference { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual TaxiRoute Route { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
