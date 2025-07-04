﻿using Microsoft.AspNetCore.Http;


namespace SWallet.Repository.Payload.Request.Student
{
    public class StudentRequest
    {
        public string? CampusId { get; set; }

        public IFormFile? StudentCardFront { get; set; }

        public string? FullName { get; set; }

        public string? Code { get; set; }
        public string? InviteCode { get; set; }

        public int? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string Address { get; set; } = null!;
    }
}
