using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace SWallet.Repository.Payload.Request.Campaign;

public class CreateCampaignModel
{
    [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
    public string BrandId { get; set; }

    [Required(ErrorMessage = "Loại chiến dịch là bắt buộc")]
    public string TypeId { get; set; }

    [Required(ErrorMessage = "Tên chiến dịch là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài tên chiến dịch từ 3 đến 255 ký tự")]
    public string CampaignName { get; set; }

    public IFormFile Image { get; set; }

    [Required(ErrorMessage = "Điều kiện là bắt buộc")]
    [StringLength(int.MaxValue, MinimumLength = 3,
            ErrorMessage = "Độ dài của điều kiện phải từ 3 ký tự trở lên")]
    public string Condition { get; set; }

    public string Link { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
    public DateOnly? StartOn { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
    public DateOnly? EndOn { get; set; }

    [Required(ErrorMessage = "Chi phí là bắt buộc")]
    [Range(minimum: 1, maximum: (double)decimal.MaxValue, ErrorMessage = "Chi phí phải là số dương")]
    public decimal? TotalIncome { get; set; }

    [Required(ErrorMessage = "Mô tả là bắt buộc")]
    [StringLength(int.MaxValue, MinimumLength = 3,
            ErrorMessage = "Độ dài mô tả phải từ 3 ký tự trở lên")]
    public string Description { get; set; }


    [Required(ErrorMessage = "Danh sách cửa hàng là bắt buộc")]
    public ICollection<CreateCampaignStoreModel> CampaignStores { get; set; }

    [Required(ErrorMessage = "Danh sách cơ sở là bắt buộc")]
    public ICollection<CreateCampaignCampusModel> CampaignCampuses { get; set; }

    [Required(ErrorMessage = "Chi tiết chiến dịch là bắt buộc")]
    public ICollection<CreateCampaignDetailModel> CampaignDetails { get; set; }
}
