using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.WishList
{
    public class WishListUpdateModel
    {
        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
        public string BrandId { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
