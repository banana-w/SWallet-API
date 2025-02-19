using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Area
{
    public class AreaRequest
    {
        [Required(ErrorMessage = "Tên khu vực là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài tên khu vực từ 3 đến 255 ký tự")]
        public string AreaName { get; set; }
        public IFormFile? Image { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
