using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Wallet
{
    public class WalletRequest
    {
        public string? CampaignId { get; set; }

        public string? StudentId { get; set; }

        public string? BrandId { get; set; }

        public string? CampusId { get; set; }

        /// <summary>
        /// Green = 1, Red = 2
        /// </summary>
        [Required(ErrorMessage = "Loại ví là bắt buộc")]
        public int? Type { get; set; }

        public decimal? Balance { get; set; }

        public string Description { get; set; }

        public bool? State { get; set; }
    }
}
