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

        [Display(Name = "Nhân viên")]
        [Description("Nhân viên quản lí trạm")]
        Lecturer = 2,

        [Display(Name = "Thương hiệu")]
        [Description("Quản lí thương hiệu")]
        Brand = 3,

        [Display(Name = "Cửa hàng")]
        [Description("Quản lí chi nhánh cửa hàng")]
        Store = 4,

        [Display(Name = "Sinh viên")]
        [Description("Sinh viên của các trường đại học")]
        Student = 5
    }
    public enum StudentState
    {
        [Display(Name = "Chờ duyệt")]
        [Description("Trạng thái chờ duyệt bởi quản trị viên")]
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
}
