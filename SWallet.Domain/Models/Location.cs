using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Location
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Address { get; set; }

    public decimal? Latitue { get; set; }

    public decimal? Longtitude { get; set; }

    public string? Qrcode { get; set; }
}
