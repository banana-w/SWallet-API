using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class PointPackage
{
    public string Id { get; set; } = null!;

    public string? PackageName { get; set; }

    public int? Point { get; set; }

    public decimal? Price { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public bool? Status { get; set; }
}
