using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Account
{
    public class UpdateAdminModel
    {
        [Required(ErrorMessage = "Tên đầy đủ là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài của họ tên từ 3 đến 255 ký tự")]
        public string FullName { get; set; }

        public IFormFile Avatar { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
