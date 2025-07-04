﻿using Microsoft.AspNetCore.Http;
using SWallet.Repository.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request
{
    public class ChallengeRequest
    {
        [Required(ErrorMessage = "Loại thử thách là bắt buộc")]
        public int? Type { get; set; }
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "Tên của thử thách là bắt buộc")]
        [StringLength(255, MinimumLength = 3,
                ErrorMessage = "Độ dài tên thử thách từ 3 đến 255 ký tự")]
        public string ChallengeName { get; set; }

        [Required(ErrorMessage = "Điểm thưởng là bắt buộc")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Điểm thưởng phải dương")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Điểm thưởng phải là số thực sau dấu phẩy 2 chữ số")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Điều kiện là bắt buộc")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Điều kiện phải dương")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Điều kiện phải là số thực sau dấu phẩy 2 chữ số")]
        public decimal? Condition { get; set; }

        [ValidExtension(new[] { ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".svg", ".webp" })]
        public IFormFile Image { get; set; }

        public string Description { get; set; }
    }
}
