using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class CampaignMajor
{
    public string Id { get; set; } = null!;

    public string CampaignId { get; set; } = null!;

    public string MajorId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Major Major { get; set; } = null!;
}
