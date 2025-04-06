using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class ActivityTransaction
{
    public string Id { get; set; } = null!;

    public string ActivityId { get; set; } = null!;

    public string? WalletId { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Rate { get; set; }

    public string Description { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Wallet? Wallet { get; set; }
}
