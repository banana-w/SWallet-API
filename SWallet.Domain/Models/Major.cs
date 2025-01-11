using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Major
{
    public string Id { get; set; } = null!;

    public string MajorName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<CampaignMajor> CampaignMajors { get; set; } = new List<CampaignMajor>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
