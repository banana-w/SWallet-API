using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class RequestTransaction
{
    public string Id { get; set; } = null!;

    public string WalletId { get; set; } = null!;

    public string RequestId { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal? Rate { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual Wallet Wallet { get; set; } = null!;
}
