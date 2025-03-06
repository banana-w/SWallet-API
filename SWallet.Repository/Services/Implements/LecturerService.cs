using AutoMapper;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class LecturerService : BaseService<LecturerService>, ILecturerService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public LecturerService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<LecturerService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Lecturer, LecturerResponse>();
                cfg.CreateMap<CreateLecturerModel, Lecturer>();
                cfg.CreateMap<UpdateLecturerModel, Lecturer>();
                cfg.CreateMap<Account, CreateLecturerModel>()
                .ReverseMap()
                .ForMember(p => p.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(p => p.IsVerify, opt => opt.MapFrom(src => true))
                .ForMember(p => p.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.DateVerified, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.Status, opt => opt.MapFrom(src => true));
            }));
        }

        public async Task<LecturerResponse> CreateLecturerAccount(CreateLecturerModel lecturer)
        {

            var newLecturer = new Lecturer
            {
                Id = Ulid.NewUlid().ToString(),
                AccountId = lecturer.AccountId,
                FullName = lecturer.FullName,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                State = true,
                Status = true
            };
            await _unitOfWork.GetRepository<Lecturer>().InsertAsync(newLecturer);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new LecturerResponse
                {
                    Id = newLecturer.Id,
                    AccountId = newLecturer.AccountId,
                    FullName = newLecturer.FullName,
                    DateCreated = newLecturer.DateCreated,
                    DateUpdated = newLecturer.DateUpdated,
                    State = newLecturer.State,
                    Status = newLecturer.Status

                };
            }
            throw new ApiException("Create Lecturer Fail", 400, "BAD_REQUEST");
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<LecturerResponse> GetLecturerById(string id)
        {
            var area = await _unitOfWork.GetRepository<Lecturer>().SingleOrDefaultAsync(
                selector: x => new LecturerResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,                    
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<IPaginate<LecturerResponse>> GetLecturers(string searchName, int page, int size)
        {
            Expression<Func<Lecturer, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.FullName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Lecturer>().GetPagingListAsync(
                selector: x => new LecturerResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status

                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<LecturerResponse> UpdateLecturer(string id, UpdateLecturerModel lecturer)
        {
            var updateLecturer = await _unitOfWork.GetRepository<Lecturer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updateLecturer == null)
            {
                throw new ApiException("Lecturer not found", 404, "NOT_FOUND");
            }

            updateLecturer.FullName = lecturer.FullName;
            updateLecturer.DateUpdated = DateTime.Now;
            updateLecturer.State = lecturer.State;
            updateLecturer.Status = lecturer.Status;
            _unitOfWork.GetRepository<Lecturer>().UpdateAsync(updateLecturer);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new LecturerResponse
                {
                    Id = updateLecturer.Id,
                    AccountId = updateLecturer.AccountId,
                    FullName = updateLecturer.FullName,
                    DateCreated = updateLecturer.DateCreated,
                    DateUpdated = updateLecturer.DateUpdated,
                    State = updateLecturer.State,
                    Status = updateLecturer.Status
                };
            }
            throw new ApiException("Update Lecturer Fail", 400, "BAD_REQUEST");
        }
    }
}
