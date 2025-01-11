using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Order
{
    public string Id { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string StationId { get; set; } = null!;

    public decimal? Amount { get; set; }

    public DateTime? DateCreated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderState> OrderStates { get; set; } = new List<OrderState>();

    public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

    public virtual Station Station { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
