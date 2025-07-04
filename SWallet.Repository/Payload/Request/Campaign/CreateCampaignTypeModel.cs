﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace SWallet.Repository.Payload.Request.Campaign;

public class CreateCampaignTypeModel
{
    [Required(ErrorMessage = "Tên loại là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Tên loại có độ dài từ 3 đến 255 ký tự")]
    public string TypeName { get; set; }
    public int? Duration { get; set; } = 0;
    public int? Coin { get; set; } = 0;
    public IFormFile Image { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
