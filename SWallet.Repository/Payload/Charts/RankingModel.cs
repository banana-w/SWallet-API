﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Charts
{
    public class RankingModel
    {
        public int? Rank { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal? Value { get; set; }
    }
}