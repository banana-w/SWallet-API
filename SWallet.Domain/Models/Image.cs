using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Image
{
    public string Id { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public bool? IsCover { get; set; }

    public DateTime? DateCreated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Product Product { get; set; } = null!;
}
