using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.QRCodeResponse
{
    public class ScanQRCodeResponse
    {
        public string StudentId { get; set; }
        public decimal PointsTransferred { get; set; }
        public decimal NewBalance { get; set; }
    }
}
