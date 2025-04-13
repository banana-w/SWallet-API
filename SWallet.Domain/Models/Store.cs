using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Store
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string AreaId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public TimeOnly? OpeningHours { get; set; }

    public TimeOnly? ClosingHours { get; set; }

    public string File { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Area Area { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CampaignStore> CampaignStores { get; set; } = new List<CampaignStore>();
}
