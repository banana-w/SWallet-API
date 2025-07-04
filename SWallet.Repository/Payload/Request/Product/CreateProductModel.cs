﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SWallet.Repository.Payload.Request.Product;

public class CreateProductModel
{
    [Required(ErrorMessage = "Thể loại sản phẩm là bắt buộc")]
    public string CategoryId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(255, MinimumLength = 3,
            ErrorMessage = "Độ dài tên sản phẩm từ 3 đến 255 ký tự")]
    public string ProductName { get; set; }

    //public List<IFormFile> ProductImages { get; set; }

    [Required(ErrorMessage = "Giá cả là bắt buộc")]
    [Range(minimum: 1, maximum: double.MaxValue, ErrorMessage = "Giá cả phải là số dương")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá cả phải là số thực sau dấu phẩy 2 chữ số")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Trọng lượng là bắt buộc")]
    [Range(minimum: 0, maximum: double.MaxValue, ErrorMessage = "Trọng lượng phải là số dương")]
    public decimal? Weight { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc")]
    [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
    public int? Quantity { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public bool? State { get; set; }
}
