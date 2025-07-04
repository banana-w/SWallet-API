﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.QRCodeRequest
{
    public class GenerateQRCodeRequest
    {
        public string LecturerId { get; set; }
        public int Points { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime StartOnTime { get; set; }
        public int AvailableHours { get; set; }
        public double Longitude { get; set; } // Thêm kinh độ
        public double Latitude { get; set; }  // Thêm vĩ độ
        public int MaxUsageCount { get; set; }
    }
}
