using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.QRCodeResponse
{
    public class QRCodeResponse
    {
        public string QRCodeData { get; set; } // Dữ liệu mã QR (JSON hoặc chuỗi)
        public string QRCodeImageUrl { get; set; } // Đường dẫn đến ảnh mã QR
    }
}
