﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Voucher
{
    public class VoucherRequest
    {
        [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
        public string BrandId { get; set; }

        [Required(ErrorMessage = "Loại khuyến mãi là bắt buộc")]
        public string TypeId { get; set; }

        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
                ErrorMessage = "Độ dài tên khuyến mãi từ 3 đến 255 ký tự")]
        public string VoucherName { get; set; }

        [Required(ErrorMessage = "Chi phí là bắt buộc")]
        [Range(minimum: 1, maximum: long.MaxValue, ErrorMessage = "Chi phí phải là số dương")]
        public long? Price { get; set; }

        [Required(ErrorMessage = "Tỷ lệ là bắt buộc")]
        [Range(minimum: 1, maximum: double.MaxValue, ErrorMessage = "Tỷ lệ phải là số dương")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Tỷ lệ phải là số thực sau dấu phẩy 2 chữ số")]
        public decimal? Rate { get; set; }

        [Required(ErrorMessage = "Điều kiện là bắt buộc")]
        [StringLength(int.MaxValue, MinimumLength = 3,
                ErrorMessage = "Độ dài của điều kiện phải từ 3 ký tự trở lên")]
        public string Condition { get; set; }

        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(int.MaxValue, MinimumLength = 3,
                ErrorMessage = "Độ dài mô tả phải từ 3 ký tự trở lên")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
