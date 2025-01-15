using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Domain.Models;


public enum CampaignState
{
    [Display(Name = "Chờ duyệt")]
    [Description("Trạng thái chờ duyệt bởi quản trị viên")]
    Pending = 1,

    [Display(Name = "Từ chối")]
    [Description("Từ chối phê duyệt chiến dịch")]
    Rejected = 2,

    [Display(Name = "Hoạt động")]
    [Description("Trạng thái hoạt động trên các nền tảng")]
    Active = 3,

    [Display(Name = "Không hoạt động")]
    [Description("Trạng thái ngừng hoạt động trên các nền tảng")]
    Inactive = 4,

    [Display(Name = "Kết thúc")]
    [Description("Trạng thái ngừng hoàn toàn trên các nền tảng")]
    Finished = 5,

    [Display(Name = "Đóng")]
    [Description("Trạng thái kết thúc")]
    Closed = 6,

    [Display(Name = "Hủy")]
    [Description("Trạng thái hủy bỏ")]
    Cancelled = 7
}


public class CampaignActivity
{
    [Key]
    [Column("id", TypeName = "char(26)")]
    public string Id { get; set; } = null!;

    [Column("campaign_id", TypeName = "char(26)")]
    public string CampaignId { get; set; } = null!;

    [Column("state",
    TypeName = "enum('Pending', 'Rejected', 'Active', 'Inactive', 'Finished', 'Closed', 'Cancelled')")]
    public CampaignState? State { get; set; }

    [Column("date_created")]
    public DateTime? DateCreated { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; } = null!;

    [Column("status", TypeName = "bit(1)")]
    public bool? Status { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;
}
