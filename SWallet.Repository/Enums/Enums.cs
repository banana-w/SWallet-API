using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Enums
{
    public enum Role
    {
        [Display(Name = "Quản trị viên")]
        [Description("Quản trị viên của hệ thống")]
        Admin = 1,

        [Display(Name = "Giáo viên")]
        [Description("Giáo viên/Giảng viên")]
        Lecturer = 2,

        [Display(Name = "Thương hiệu")]
        [Description("Quản lí thương hiệu")]
        Brand = 3,

        [Display(Name = "Cửa hàng")]
        [Description("Quản lí chi nhánh cửa hàng")]
        Store = 4,

        [Display(Name = "Sinh viên")]
        [Description("Sinh viên của các trường đại học")]
        Student = 5,
            
        [Display(Name = "Campus")]
        [Description("Campus")]
        Campus = 5
    }
    public enum StudentState
    {
        [Display(Name = "Chờ duyệt")]
        [Description("Trạng thái chờ duyệt")]
        Pending = 1,

        [Display(Name = "Hoạt động")]
        [Description("Trạng thái hoạt động trên các nền tảng")]
        Active = 2,

        [Display(Name = "Không hoạt động")]
        [Description("Trạng thái ngừng hoạt động trên các nền tảng")]
        Inactive = 3,

        [Display(Name = "Từ chối")]
        [Description("Từ chối xác nhận tài khoản")]
        Rejected = 4,
    }
    public enum WalletType
    {
        [Display(Name = "Ví xanh")]
        [Description("Ví xanh dành cho sinh viên")]
        Green = 1,
        [Display(Name = "Ví đỏ")]
        [Description("Ví đỏ dành cho giáo viên")]
        Red = 2
    }

    public enum ActivityType
    {
        [Display(Name = "Mua")]
        [Description("Mua khuyến mãi")]
        Buy = 1,

        [Display(Name = "Sử dụng")]
        [Description("Sử dụng khuyến mãi")]
        Use = 2,
    }

    public enum ChallengeType
    {
        [Display(Name = "Thường")]
        [Description("Thách thức thường")]
        Achievement = 1,
        [Display(Name = "Ngày")]
        [Description("Thách thức theo ngày")]
        Daily = 2,
        [Display(Name = "Tuần")]
        [Description("Thách thức theo tuần")]
        Weekly = 3,
        [Display(Name = "Tháng")]
        [Description("Thách thức theo tháng")]
        Monthly = 4,
    }
}
