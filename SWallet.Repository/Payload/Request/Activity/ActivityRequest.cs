using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Activity
{
    public class ActivityRequest
    {
        public string StoreId { get; set; }

        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "voucher item id là bắt buộc")]
        public string VoucherItemId { get; set; }

        /// <summary>
        /// Buy = 1, Use = 2
        /// </summary>
        [Required(ErrorMessage = "Loại hoạt động là bắt buộc")]
        public ActivityType? Type { get; set; }
        public decimal? Cost { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool? State { get; set; }
    }
}
