using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Station
{
    public string Id { get; set; } = null!;

    public string StationName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public TimeOnly? OpeningHours { get; set; }

    public TimeOnly? ClosingHours { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public int? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
