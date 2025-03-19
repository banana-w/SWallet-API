using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class ChallengeTransaction
{
    public string Id { get; set; } = null!;

    public string WalletId { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? ChallengeId { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? DateCreated { get; set; }

    public string Description { get; set; } = null!;

    public bool State { get; set; }

    public bool? Status { get; set; }

    public virtual StudentChallenge? StudentChallenge { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
