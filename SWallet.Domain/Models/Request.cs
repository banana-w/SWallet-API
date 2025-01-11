using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Request
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string AdminId { get; set; } = null!;

    public decimal? Amount { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<RequestTransaction> RequestTransactions { get; set; } = new List<RequestTransaction>();
}
