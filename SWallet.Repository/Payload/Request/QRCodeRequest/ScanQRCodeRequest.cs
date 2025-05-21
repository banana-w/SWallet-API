using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.QRCodeRequest
{
    public class ScanQRCodeRequest
    {
        public string QRCodeJson { get; set; } // Chuỗi JSON từ QRCode
        public string StudentId { get; set; }  // ID của Student quét QRCode
        public double Longitude { get; set; } // Thêm kinh độ của sinh viên
        public double Latitude { get; set; }  // Thêm vĩ độ của sinh viên
    }
}
