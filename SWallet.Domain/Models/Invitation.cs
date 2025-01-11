using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Invitation
{
    public string Id { get; set; } = null!;

    public string InviterId { get; set; } = null!;

    public string InviteeId { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public string Description { get; set; } = null!;

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual Student Invitee { get; set; } = null!;

    public virtual Student Inviter { get; set; } = null!;
}
