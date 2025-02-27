using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Campaign
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string TypeId { get; set; } = null!;

    public string CampaignName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string ImageName { get; set; } = null!;

    public string File { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string Condition { get; set; } = null!;

    public string Link { get; set; } = null!;

    public DateOnly? StartOn { get; set; }

    public DateOnly? EndOn { get; set; }

    public int? Duration { get; set; }

    public decimal? TotalIncome { get; set; }

    public decimal? TotalSpending { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CampaignCampus> CampaignCampuses { get; set; } = new List<CampaignCampus>();

    public virtual ICollection<CampaignDetail> CampaignDetails { get; set; } = new List<CampaignDetail>();

    public virtual ICollection<CampaignStore> CampaignStores { get; set; } = new List<CampaignStore>();

    public virtual ICollection<CampaignTransaction> CampaignTransactions { get; set; } = new List<CampaignTransaction>();

    public virtual CampaignType Type { get; set; } = null!;

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
