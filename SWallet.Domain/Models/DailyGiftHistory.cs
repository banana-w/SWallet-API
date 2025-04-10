using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class DailyGiftHistory
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public DateTime? CheckInDate { get; set; }

    public int? Streak { get; set; }

    public int? Points { get; set; }
}
