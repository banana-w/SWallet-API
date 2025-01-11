using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class OrderDetail
{
    public string Id { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
