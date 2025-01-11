using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Reward
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public decimal? Amount { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<RewardTransaction> RewardTransactions { get; set; } = new List<RewardTransaction>();

    public virtual Store Store { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
