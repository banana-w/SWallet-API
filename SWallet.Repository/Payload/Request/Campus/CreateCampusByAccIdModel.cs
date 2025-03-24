using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Campus
{
    public class CreateCampusByAccIdModel
    {
        [Required(ErrorMessage = "Account ID là bắt buộc")]
        public string AccountId { get; set; }

        [Required(ErrorMessage = "Khu vực là bắt buộc")]
        public string AreaId { get; set; }

        [Required(ErrorMessage = "Tên trường là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
                ErrorMessage = "Độ dài tên trường từ 3 đến 255 ký tự")]
        public string CampusName { get; set; }

        public string Address { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Link { get; set; }

        public IFormFile Image { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
