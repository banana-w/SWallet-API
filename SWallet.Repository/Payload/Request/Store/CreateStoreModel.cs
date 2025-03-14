using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SWallet.Repository.Payload.Request.Store;

public class CreateStoreModel
{

    [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
    public string BrandId { get; set; }


    [Required(ErrorMessage = "Khu vực là bắt buộc")]
    public string AreaId { get; set; }

    [Required(ErrorMessage = "Tên cửa hàng là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài tên cửa hàng từ 3 đến 255 ký tự")]
    public string StoreName { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    public string Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Required(ErrorMessage = "Email là bắt buộc")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài của địa chỉ từ 3 đến 255 ký tự")]
    public string Address { get; set; }

    public IFormFile Avatar { get; set; }

    public TimeOnly? OpeningHours { get; set; }

    public TimeOnly? ClosingHours { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
