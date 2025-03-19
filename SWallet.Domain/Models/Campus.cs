using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Campus
{
    public string Id { get; set; } = null!;

    public string? AccountId { get; set; }

    public string AreaId { get; set; } = null!;

    public string CampusName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? LinkWebsite { get; set; }

    public string? Image { get; set; }

    public string? FileName { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Description { get; set; }

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<CampaignCampus> CampaignCampuses { get; set; } = new List<CampaignCampus>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
