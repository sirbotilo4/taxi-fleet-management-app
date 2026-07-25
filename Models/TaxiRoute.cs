using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class TaxiRoute
{
    public int RouteId { get; set; }

    public string Name { get; set; } = null!;

    public string StartPoint { get; set; } = null!;

    public string EndPoint { get; set; } = null!;

    public decimal StandardFare { get; set; }

    public string RouteType { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
