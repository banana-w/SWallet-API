using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class PointPurchaseHistory
{
    public string Id { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public string PointPackageId { get; set; } = null!;

    public int Points { get; set; }

    public decimal Amount { get; set; }

    public long PaymentId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual PointPackage PointPackage { get; set; } = null!;
}
