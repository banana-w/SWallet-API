using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Campus
{
    public string Id { get; set; } = null!;

    public string UniversityId { get; set; } = null!;

    public string AreaId { get; set; } = null!;

    public string CampusName { get; set; } = null!;

    public TimeOnly? OpeningHours { get; set; }

    public TimeOnly? ClosingHours { get; set; }

    public string Address { get; set; } = null!;

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

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<CampaignCampus> CampaignCampuses { get; set; } = new List<CampaignCampus>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual University University { get; set; } = null!;
}
