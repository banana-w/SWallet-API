using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Student
{
    public string Id { get; set; } = null!;

    public string MajorId { get; set; } = null!;

    public string CampusId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string StudentCardFront { get; set; } = null!;

    public string FileNameFront { get; set; } = null!;

    public string StudentCardBack { get; set; } = null!;

    public string FileNameBack { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string Address { get; set; } = null!;

    public decimal? TotalIncome { get; set; }

    public decimal? TotalSpending { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? State { get; set; }

    public bool? Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Invitation> InvitationInvitees { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationInviters { get; set; } = new List<Invitation>();

    public virtual Major Major { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual ICollection<StudentChallenge> StudentChallenges { get; set; } = new List<StudentChallenge>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
