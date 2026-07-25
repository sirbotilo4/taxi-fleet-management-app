using System;
using System.Collections.Generic;

namespace TaxiFleetManager.Models;

public partial class OwnerProfile
{
    public int OwnerId { get; set; }

    public string BusinessName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? BankName { get; set; }

    public string? AccountNumber { get; set; }

    public string? BranchCode { get; set; }
}
