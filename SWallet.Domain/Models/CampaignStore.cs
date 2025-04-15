using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class CampaignStore
{
    public string Id { get; set; } = null!;

    public string CampaignId { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
