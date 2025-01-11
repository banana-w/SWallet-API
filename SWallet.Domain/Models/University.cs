using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class University
{
    public string Id { get; set; } = null!;

    public string UniversityName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Campus> Campuses { get; set; } = new List<Campus>();
}
