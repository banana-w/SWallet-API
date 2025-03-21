using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Lecturer
{
    public string Id { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CampusLecturer> CampusLecturers { get; set; } = new List<CampusLecturer>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
