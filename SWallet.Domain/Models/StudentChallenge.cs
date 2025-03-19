using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class StudentChallenge
{
    public string ChallengeId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal? Current { get; set; }

    public decimal? Condition { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual ICollection<ChallengeTransaction> ChallengeTransactions { get; set; } = new List<ChallengeTransaction>();

    public virtual Student Student { get; set; } = null!;
}
