using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal? Price { get; set; }

    public decimal? Weight { get; set; }

    public int? Quantity { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
