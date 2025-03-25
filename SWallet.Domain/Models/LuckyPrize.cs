using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class LuckyPrize
{
    public int Id { get; set; }

    public string? PrizeName { get; set; }

    public decimal? Probability { get; set; }

    public int? Quantity { get; set; }

    public bool? Status { get; set; }

    public int? Value { get; set; }
}
