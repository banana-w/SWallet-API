using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Charts
{
    public class TitleStoreModel
    {
        public long? NumberOfParticipants { get; set; }
        public long? NumberOfBonuses { get; set; }
        public decimal? AmountOfBonuses { get; set; }
        public decimal? BrandBalance { get; set; }
        public int? BrandWalletTypeId { get; set; } = (int)WalletType.Green;
        public string BrandWalletType { get; set; } = WalletType.Green.ToString();
        //public string BrandWalletTypeName { get; set; } = WalletType.Green.GetDisplayName();
    }
}
