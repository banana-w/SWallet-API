using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class CampaignActivity
{
    public string Id { get; set; } = null!;

    public string CampaignId { get; set; } = null!;

    public int? State { get; set; }

    public DateTime? DateCreated { get; set; }

    public string Description { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;
}
