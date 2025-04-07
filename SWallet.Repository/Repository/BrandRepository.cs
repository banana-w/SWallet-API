using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Repository
{
    public interface IBrandRepository
    {
        List<Brand> GetRanking(int limit);

        long CountBrand();
    }

    public class BrandRepository : IBrandRepository
    {
        private readonly SwalletDbContext swalletDB;

        public BrandRepository(SwalletDbContext swalletDB)
        {
            this.swalletDB = swalletDB;
        }

        public long CountBrand()
        {
            long count = 0;
            try
            {
                var db = swalletDB;
                count = db.Brands.Where(c => (bool)c.Status).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public List<Brand> GetRanking(int limit)
        {
            List<Brand> result = new();
            try
            {
                var db = swalletDB;
                result.AddRange(db.Brands.Where(
                    b => (bool)b.Status).OrderByDescending(
                    b => b.TotalSpending).Take(limit).Include(b => b.Account));
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

    }
}
