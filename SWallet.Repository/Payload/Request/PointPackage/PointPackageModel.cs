using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.PointPackage
{
    public class PointPackageModel
    {
        [Required(ErrorMessage = "Tên gói điểm là bắt buộc")]
        [StringLength(255, MinimumLength = 6,
            ErrorMessage = "Độ dài tên gói điểm từ 6 đến 255 ký tự")]
        public string? PackageName { get; set; }

        [Required(ErrorMessage = "Điểm là bắt buộc")]
        public int? Point { get; set; }

        [Required(ErrorMessage = "Giá của gói điểm là bắt buộc")]
        public decimal? Price { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public bool? Status { get; set; }
    }
}
