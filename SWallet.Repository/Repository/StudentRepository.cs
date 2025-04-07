using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SWallet.Repository.Repository.IStudentRepository;

namespace SWallet.Repository.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SwalletDbContext swalletDB;

        public StudentRepository(SwalletDbContext swalletDB)
        {
            this.swalletDB = swalletDB;
        }

        public bool CheckStudentId(string id)
        {
            throw new NotImplementedException();
        }

        public long CountStudent()
        {
            throw new NotImplementedException();
        }

        public long CountStudentToday(DateOnly date)
        {
            long count = 0;
            try
            {
                var db = swalletDB;
                count = db.Students.Where(c => (bool)c.Status
                && DateOnly.FromDateTime(c.DateCreated.Value).Equals(date)).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public bool CheckCodeDuplicate(string code)
        {
            throw new NotImplementedException();
        }

        public bool CheckInviteCode(string inviteCode)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public List<Student> GetRanking(int limit)
        {
            List<Student> result = new();
            try
            {
                var db = swalletDB;
                result.AddRange(db.Students.Where(
                    s => (bool)s.Status).OrderByDescending(
                    s => s.TotalSpending).Take(limit).Include(s => s.Account));
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<StudentRanking> GetRankingByBrand(string brandId, int limit)
        {
            throw new NotImplementedException();
        }

        public List<StudentRanking> GetRankingByStation(string stationId, int limit)
        {
            throw new NotImplementedException();
        }

        public List<StudentRanking> GetRankingByStore(string storeId, int limit)
        {
            throw new NotImplementedException();
        }
    }
}
