using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Voucher
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string TypeId { get; set; } = null!;

    public string VoucherName { get; set; } = null!;

    public decimal? Price { get; set; }

    public decimal? Rate { get; set; }

    public string Condition { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string ImageName { get; set; } = null!;

    public string File { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CampaignDetail> CampaignDetails { get; set; } = new List<CampaignDetail>();

    public virtual VoucherType Type { get; set; } = null!;

    public virtual ICollection<VoucherItem> VoucherItems { get; set; } = new List<VoucherItem>();
}
