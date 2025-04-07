using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Charts
{
    public class LineChartModel
    {
        public decimal? Green { get; set; }
        public decimal? Red { get; set; }
        public DateOnly? Date { get; set; }
    }
}
