using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Student
{
    public string Id { get; set; } = null!;

    public string? CampusId { get; set; }

    public string AccountId { get; set; } = null!;

    public string? StudentCardFront { get; set; }

    public string? FileNameFront { get; set; }

    public string? StudentCardBack { get; set; }

    public string? FileNameBack { get; set; }

    public string? FullName { get; set; }

    public string? Code { get; set; }

    public int? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public decimal? TotalIncome { get; set; }

    public decimal? TotalSpending { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? State { get; set; }

    public bool? Status { get; set; }

    public string? StudentEmail { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Invitation> InvitationInvitees { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationInviters { get; set; } = new List<Invitation>();

    public virtual ICollection<SpinHistory> SpinHistories { get; set; } = new List<SpinHistory>();

    public virtual ICollection<StudentChallenge> StudentChallenges { get; set; } = new List<StudentChallenge>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
