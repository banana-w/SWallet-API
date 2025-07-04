﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Campaign
{
    public class CampaignResponseExtra
    {
        public string Id { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandAcronym { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string CampaignName { get; set; }
        public string Image { get; set; }
        public string ImageName { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public string Condition { get; set; }
        public string Link { get; set; }
        public DateOnly? StartOn { get; set; }
        public DateOnly? EndOn { get; set; }
        public int? Duration { get; set; }
        public decimal? TotalIncome { get; set; }
        public decimal? TotalSpending { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public IEnumerable<string> CampaignDetailId { get; set; }
        public string? BrandLogo { get; set; }
    }

    public class CampaignResponseExtraAllStatus
    {
        public string Id { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandAcronym { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string CampaignName { get; set; }
        public string Image { get; set; }
        public string ImageName { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public string Condition { get; set; }
        public string Link { get; set; }
        public DateOnly? StartOn { get; set; }
        public DateOnly? EndOn { get; set; }
        public int? Duration { get; set; }
        public decimal? TotalIncome { get; set; }
        public decimal? TotalSpending { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public IEnumerable<string> CampaignDetailId { get; set; }
        public string? BrandLogo { get; set; }
    }
}
