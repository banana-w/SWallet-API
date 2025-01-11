using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Challenge
{
    public string Id { get; set; } = null!;

    public int? Type { get; set; }

    public string ChallengeName { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal? Condition { get; set; }

    public string Image { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<StudentChallenge> StudentChallenges { get; set; } = new List<StudentChallenge>();
}
