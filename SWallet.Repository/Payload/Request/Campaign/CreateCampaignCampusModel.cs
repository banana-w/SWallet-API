using System.ComponentModel.DataAnnotations;

namespace SWallet.Repository.Payload.Request.Campaign;


public class CreateCampaignCampusModel
{
    [Required(ErrorMessage = "Cơ sở là bắt buộc")]
    public string CampusId { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
