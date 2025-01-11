using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class VoucherItem
{
    public string Id { get; set; } = null!;

    public string VoucherId { get; set; } = null!;

    public string CampaignDetailId { get; set; } = null!;

    public string VoucherCode { get; set; } = null!;

    public int? Index { get; set; }

    public bool? IsLocked { get; set; }

    public bool? IsBought { get; set; }

    public bool? IsUsed { get; set; }

    public DateOnly? ValidOn { get; set; }

    public DateOnly? ExpireOn { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateIssued { get; set; }

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual CampaignDetail CampaignDetail { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;
}
