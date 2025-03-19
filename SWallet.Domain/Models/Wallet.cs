using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Wallet
{
    public string Id { get; set; } = null!;

    public string? LecturerId { get; set; }

    public string? CampusId { get; set; }

    public string? CampaignId { get; set; }

    public string? StudentId { get; set; }

    public string? BrandId { get; set; }

    public int? Type { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<ActivityTransaction> ActivityTransactions { get; set; } = new List<ActivityTransaction>();

    public virtual Brand? Brand { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual ICollection<CampaignTransaction> CampaignTransactions { get; set; } = new List<CampaignTransaction>();

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<ChallengeTransaction> ChallengeTransactions { get; set; } = new List<ChallengeTransaction>();

    public virtual Lecturer? Lecturer { get; set; }

    public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

    public virtual ICollection<RequestTransaction> RequestTransactions { get; set; } = new List<RequestTransaction>();

    public virtual ICollection<RewardTransaction> RewardTransactions { get; set; } = new List<RewardTransaction>();

    public virtual Student? Student { get; set; }
}
