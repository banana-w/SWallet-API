using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class SpinHistory
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public DateOnly Date { get; set; }

    public int? SpinCount { get; set; }

    public int? BonusSpins { get; set; }

    public virtual Student Student { get; set; } = null!;
}
