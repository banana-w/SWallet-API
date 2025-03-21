using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class CampusLecturer
{
    public string Id { get; set; } = null!;

    public string? CampusId { get; set; }

    public string? LecturerId { get; set; }

    public bool? Status { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual Lecturer? Lecturer { get; set; }
}
