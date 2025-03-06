using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SWallet.Repository.Payload.Request.Campus;

public class UpdateCampusModel
{

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
