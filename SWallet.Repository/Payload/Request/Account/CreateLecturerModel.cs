using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Account
{
    public class CreateLecturerModel
    {
        public string AccountId { get; set; } = null!;

        [Required(ErrorMessage = "Tên giảng viên là bắt buộc")]
        public string FullName { get; set; } = null!;

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
