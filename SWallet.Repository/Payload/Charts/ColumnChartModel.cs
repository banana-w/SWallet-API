using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Charts
{
    public class ColumnChartModel
    {
        public decimal? Value { get; set; }
        public DateOnly? Date { get; set; }
    }
}
