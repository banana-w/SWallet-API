using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Brand
{
    public class CreateBrandGoogleModel
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc")]
        public string AccountId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
                ErrorMessage = "Độ dài tên thương hiệu từ 3 đến 255 ký tự")]
        public string BrandName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string Email { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
