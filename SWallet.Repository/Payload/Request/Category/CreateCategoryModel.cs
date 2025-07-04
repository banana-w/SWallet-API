﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace SWallet.Repository.Payload.Request.Category;

public class CreateCategoryModel
{
    [Required(ErrorMessage = "Tên thể loại là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài tên thể loại từ 3 đến 255 ký tự")]
    public string CategoryName { get; set; }

    public IFormFile Image { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
