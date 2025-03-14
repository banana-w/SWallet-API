using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Voucher
{
    public class VoucherTypeRequest
    {
        [Required(ErrorMessage = "Tên loại khuyến mãi là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Tên loại khuyến mãi có độ dài từ 3 đến 255 ký tự")]
        public string TypeName { get; set; }

        public IFormFile Image { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
