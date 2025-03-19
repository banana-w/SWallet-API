using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.PointPackage;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class PointPackageService : BaseService<PointPackageService>, IPointPackageService
    {
        IVnpay _vnPay;

        public PointPackageService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<PointPackageService> logger, IVnpay vnPay) : base(unitOfWork, logger)
        {
            _vnPay = vnPay;
        }

        public async Task<PointPackage> CreatePointPackage(PointPackageModel package)
        {
            var newPackage = new PointPackage
            {
                Id = Ulid.NewUlid().ToString(),
                Point = package.Point,
                Price = package.Price,
                PackageName = package.PackageName,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = true
            };

            await _unitOfWork.GetRepository<PointPackage>().InsertAsync(newPackage);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new PointPackage
                {
                    Id = newPackage.Id,
                    PackageName = newPackage.PackageName,
                    Point = newPackage.Point,
                    Price = newPackage.Price,
                    DateCreated = newPackage.DateCreated,
                    DateUpdated = newPackage.DateUpdated,
                    Status = newPackage.Status

                };
                
            }

           throw new ApiException("Create Package Fail", 400, "BAD_REQUEST"); ;
        }


        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<PointPackage>> GetPointPackage(string searchName, int page, int size)
        {
            Expression<Func<PointPackage, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.PackageName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<PointPackage>().GetPagingListAsync(
                selector: x => new PointPackage
                {
                    Id = x.Id,
                    Point = x.Point,
                    PackageName = x.PackageName,
                    Price = x.Price,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Status = x.Status

                },
                predicate: filterQuery,
                page: page, 
                size: size);
            return areas;
        }

        public async Task<PointPackage> GetPointPackageById(string id)
        {
            var area = await _unitOfWork.GetRepository<PointPackage>().SingleOrDefaultAsync(
               selector: x => new PointPackage
               {
                   Id = x.Id,
                   Point = x.Point,
                   Price = x.Price,
                   DateCreated = x.DateCreated,
                   PackageName = x.PackageName,
                   DateUpdated = x.DateUpdated,
                   Status = x.Status
               },
               predicate: x => x.Id == id);
            return area;
        }

        public async Task<PointPackage> UpdatePointPackage(string id, PointPackageModel package)
        {
            var updatePackage = await _unitOfWork.GetRepository<PointPackage>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updatePackage == null)
            {
                throw new ApiException("Package not found", 404, "NOT_FOUND");
            }
            
            updatePackage.Point = package.Point;
            updatePackage.PackageName = package.PackageName;
            updatePackage.Price = package.Price;
            updatePackage.DateUpdated = DateTime.Now;
            updatePackage.DateCreated = DateTime.Now;
            updatePackage.Status = package.Status;

            _unitOfWork.GetRepository<PointPackage>().UpdateAsync(updatePackage);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new PointPackage
                {
                    Id = updatePackage.Id,
                    Point = updatePackage.Point,
                    PackageName = updatePackage.PackageName,
                    Price = updatePackage.Price,
                    DateCreated = updatePackage.DateCreated,
                    DateUpdated = updatePackage.DateUpdated,
                    Status = updatePackage.Status
                };
            }
            throw new ApiException("Update Point Package Fail", 400, "BAD_REQUEST");
        }
    }
}
