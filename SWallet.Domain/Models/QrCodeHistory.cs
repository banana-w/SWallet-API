using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class QrCodeHistory
{
    public string Id { get; set; } = null!;

    public string LectureId { get; set; } = null!;

    public int Points { get; set; }

    public DateTime StartOnTime { get; set; }

    public DateTime ExpirationTime { get; set; }

    public string? QrCodeData { get; set; }

    public string? QrCodeImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
