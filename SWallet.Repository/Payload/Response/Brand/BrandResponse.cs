using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Brand
{
    public class BrandResponse
    {
        public string Id { get; set; } = null!;

        public string AccountId { get; set; } = null!;

        public string BrandName { get; set; } = null!;

        public string Acronym { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string CoverPhoto { get; set; } = null!;

        public string CoverFileName { get; set; } = null!;

        public string Link { get; set; } = null!;

        public TimeOnly? OpeningHours { get; set; }

        public TimeOnly? ClosingHours { get; set; }

        public decimal? TotalIncome { get; set; }

        public decimal? TotalSpending { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Description { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
