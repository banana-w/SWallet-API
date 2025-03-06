using System.ComponentModel.DataAnnotations;


namespace SWallet.Repository.Payload.Request.Campaign;

public class CreateCampaignStoreModel
{
    [Required(ErrorMessage = "Cửa hàng là bắt buộc")]
    public string StoreId { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
