using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class CampaignDetail
{
    public string Id { get; set; } = null!;

    public string VoucherId { get; set; } = null!;

    public string CampaignId { get; set; } = null!;

    public decimal? Price { get; set; }

    public decimal? Rate { get; set; }

    public int? Quantity { get; set; }

    public int? FromIndex { get; set; }

    public int? ToIndex { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;

    public virtual ICollection<VoucherItem> VoucherItems { get; set; } = new List<VoucherItem>();
}
