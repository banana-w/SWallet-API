using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.QRCodeResponse
{
    public class QrCodeHistoryResponse
    {
        public string Id { get; set; }
        public string LecturerId { get; set; } = null!;
        public int Points { get; set; }
        public DateTime StartOnTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string QRCodeData { get; set; } = null!;
        public string QRCodeImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }

    }
}
