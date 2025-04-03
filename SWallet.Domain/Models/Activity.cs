using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Activity
{
    public string Id { get; set; } = null!;

    public string? StoreId { get; set; }

    public string StudentId { get; set; } = null!;

    public string VoucherItemId { get; set; } = null!;

    public int? Type { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<ActivityTransaction> ActivityTransactions { get; set; } = new List<ActivityTransaction>();

    public virtual Store? Store { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual VoucherItem VoucherItem { get; set; } = null!;
}
