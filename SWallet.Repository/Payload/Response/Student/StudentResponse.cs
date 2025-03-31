using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Student
{
    public class StudentResponse
    {
        public string Id { get; set; } = null!;

        public string? CampusId { get; set; }
        public string? CampusName { get; set; }

        public string AccountId { get; set; } = null!;

        public string? StudentCardFront { get; set; }

        public string? FileNameFront { get; set; }

        public string? StudentCardBack { get; set; }

        public string? FileNameBack { get; set; }

        public string? FullName { get; set; }
        public string? StudentEmail { get; set; }

        public decimal? CoinBalance { get; set; }

        public string? Code { get; set; }

        public int? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string Address { get; set; } = null!;

        public decimal? TotalIncome { get; set; }

        public decimal? TotalSpending { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? State { get; set; }

        public bool? Status { get; set; }
    }
}
