using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class QrcodeUsage
{
    public int Id { get; set; }

    public string? QrcodeJson { get; set; }

    public string? StudentId { get; set; }

    public DateTime? UsedAt { get; set; }
}
