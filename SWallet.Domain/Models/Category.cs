using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
