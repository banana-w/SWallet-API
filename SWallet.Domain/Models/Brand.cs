using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Brand
{
    public string Id { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public string Acronym { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string CoverPhoto { get; set; } = null!;

    public string CoverFileName { get; set; } = null!;

    public string Link { get; set; } = null!;

    public TimeOnly? OpeningHours { get; set; }

    public TimeOnly? ClosingHours { get; set; }

    public decimal? TotalIncome { get; set; }

    public decimal? TotalSpending { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
