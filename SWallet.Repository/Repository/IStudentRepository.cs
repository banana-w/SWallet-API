using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Repository
{
    public interface IStudentRepository
    {
        public record StudentRanking
        {
            public string Name { get; set; }
            public string Image { get; set; }
            public decimal? TotalSpending { get; set; }
        }

      
            bool CheckStudentId(string id);

            long CountStudent();

            long CountStudentToday(DateOnly date);

            bool CheckCodeDuplicate(string code);

            bool CheckInviteCode(string inviteCode);

            void Delete(string id);

            List<Student> GetRanking(int limit);

            List<StudentRanking> GetRankingByBrand(string brandId, int limit);

            List<StudentRanking> GetRankingByStation(string stationId, int limit);

            List<StudentRanking> GetRankingByStore(string storeId, int limit);

        
    }
}
