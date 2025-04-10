using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.DailyCheckIn
{
    public class CheckInResponse
    {
        public bool[] CheckInHistory { get; set; } // Lịch sử điểm danh (7 ngày)
        public int Streak { get; set; } // Chuỗi điểm danh
        public int Points { get; set; } // Tổng điểm
        public bool CanCheckInToday { get; set; } // Có thể điểm danh hôm nay không
        public int CurrentDayIndex { get; set; } // Ngày hiện tại trong chu kỳ

        public int RewardPoints { get; set; } // Điểm thưởng cho lần điểm danh này
    }
}
