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
        public string CampaignId { get; set; }

        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        public string StudentId { get; set; }
        /// <summary>
        /// Buy = 1, Use = 2
        /// </summary>
        public decimal? Cost { get; set; }
        public int Quantity { get; set; }   

    }
}
