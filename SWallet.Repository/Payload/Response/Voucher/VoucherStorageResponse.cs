using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Voucher
{
    public class VoucherStorageResponse
    {
        public string Id { get; set; }
        public string VoucherId { get; set; } = string.Empty;
        public string VoucherName { get; set; } = string.Empty;
        public string VoucherImage { get; set; } = string.Empty;
        public string VoucherCode { get; set; } = string.Empty;
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeImage { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandImage { get; set; }
        public string CampaignDetailId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsBought { get; set; }
        public bool? IsUsed { get; set; }
        public DateOnly? ValidOn { get; set; }
        public DateOnly? ExpireOn { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateLocked { get; set; }
        public DateTime? DateBought { get; set; }
        public DateTime? DateUsed { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? State { get; set; }
        public bool? Status { get; set; }
    }

    public class VoucherStorageGroupByBrandResponse
    {
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandImage { get; set; }
        public List<VoucherGroup> VoucherGroups { get; set; }
    }

    public class VoucherGroup
    {
        public string VoucherId { get; set; }
        public string VoucherName { get; set; }
        public string VoucherImage { get; set; }
        public int Quantity { get; set; } // Số lượng VoucherItem còn lại (chưa dùng)
        public int TotalQuantity { get; set; }
        public DateOnly? ExpireOn { get; set; }
        public string CampaignId { get; set; }
        public List<VoucherStorageResponse> Vouchers { get; set; } // Danh sách chi tiết nếu cần
    }
}
