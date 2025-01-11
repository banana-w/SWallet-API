using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Wishlist
{
    public string Id { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
