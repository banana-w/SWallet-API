using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Utils
{
    public static class TimeUtils
    {
        public static TimeZoneInfo GetVietnamTimeZone()
        {
            string timeZoneId = OperatingSystem.IsWindows()
                ? "SE Asia Standard Time"
                : "Asia/Ho_Chi_Minh";

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback nếu múi giờ không tìm thấy
                return TimeZoneInfo.Utc; // Hoặc throw exception tùy yêu cầu
            }
        }
    }
}
