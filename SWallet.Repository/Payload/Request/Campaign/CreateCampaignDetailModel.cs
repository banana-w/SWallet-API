using System.ComponentModel.DataAnnotations;


namespace SWallet.Repository.Payload.Request.Campaign;

public class CreateCampaignDetailModel
{
    [Required(ErrorMessage = "Khuyến mãi là bắt buộc")]
    public string VoucherId { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc")]
    [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Chỉ mục là bắt buộc")]
    [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Số lượng phải không âm")]
    public int? FromIndex { get; set; }

    //[Required(ErrorMessage = "Chỉ mục là bắt buộc")]
    //[Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Số lượng phải không âm")]
    //public int? ToIndex { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
