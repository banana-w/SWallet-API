﻿using System;
using System.Collections.Generic;

namespace SWallet.Domain.Models;

public partial class Account
{
    public string Id { get; set; } = null!;

    public int? Role { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Avatar { get; set; }

    public string? FileName { get; set; }

    public bool? IsVerify { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateVerified { get; set; }

    public string? Description { get; set; }

    public bool? State { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<Campus> Campuses { get; set; } = new List<Campus>();

    public virtual ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
