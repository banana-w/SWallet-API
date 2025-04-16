using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //public async Task<LecturerResponse> CreateLecturerAccount(CreateLecturerModel lecturer)
        //{

        //    var newLecturer = new Lecturer
        //    {
        //        Id = Ulid.NewUlid().ToString(),
        //        AccountId = lecturer.AccountId,
        //        FullName = lecturer.FullName,
        //        DateCreated = DateTime.Now,
        //        DateUpdated = DateTime.Now,
        //        State = true,
        //        Status = true
        //    };
        //    await _unitOfWork.GetRepository<Lecturer>().InsertAsync(newLecturer);
        //    var isSuccess = await _unitOfWork.CommitAsync() > 0;

        //    if (isSuccess)
        //    {
        //        return new LecturerResponse
        //        {
        //            Id = newLecturer.Id,
        //            AccountId = newLecturer.AccountId,
        //            FullName = newLecturer.FullName,
        //            DateCreated = newLecturer.DateCreated,
        //            DateUpdated = newLecturer.DateUpdated,
        //            State = newLecturer.State,
        //            Status = newLecturer.Status

        //        };
        //    }
        //    throw new ApiException("Create Lecturer Fail", 400, "BAD_REQUEST");
        //}

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<LecturerResponse> GetLecturerById(string id)
        {
            var lecturer = await _unitOfWork.GetRepository<Lecturer>().SingleOrDefaultAsync(
                selector: x => new LecturerResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    CampusName = x.CampusLecturers.Select(lc => lc.Campus.CampusName).ToList(), // Lấy danh sách tên campus
                    Balance = (decimal)(x.Wallets != null && x.Wallets.Any() ? x.Wallets.FirstOrDefault().Balance : 0), // Lấy Balance từ Wallet đầu tiên
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status,
                },
                predicate: x => x.Id == id,
                include: x => x
                    .Include(l => l.Wallets) // Include Wallets để lấy Balance
                    .Include(l => l.CampusLecturers)
                    .ThenInclude(lc => lc.Campus)
                    .Include(l => l.Account) // Include Account để lấy Email và Phone
            );

            if (lecturer == null)
            {
                throw new Exception($"Lecturer with ID {id} not found.");
            }

            return lecturer;
        }

        public async Task<LecturerResponse> GetLecturerByAccountId(string accountId)
        {
            var area = await _unitOfWork.GetRepository<Lecturer>().SingleOrDefaultAsync(
                selector: x => new LecturerResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    CampusName = x.CampusLecturers.Select(lc => lc.Campus.CampusName).ToList(), // Lấy danh sách tên campus
                    Balance = (decimal)(x.Wallets != null && x.Wallets.Any() ? x.Wallets.FirstOrDefault().Balance : 0), // Lấy Balance từ Wallet đầu tiên
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status,

                },
                predicate: x => x.AccountId == accountId,
                include: x => x
                    .Include(l => l.Wallets) // Include Wallets để lấy Balance
                    .Include(l => l.CampusLecturers)
                    .ThenInclude(lc => lc.Campus)
                    .Include(l => l.Account)); // Include Account để lấy Email và Phone);
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

        public async Task<IPaginate<LecturerResponse>> GetLecturersByCampusId(string campusId, string searchName, int page, int size)
        {
            Expression<Func<CampusLecturer, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.CampusId == campusId;
            }
            else
            {
                filterQuery = p => p.CampusId == campusId && p.Lecturer.FullName.Contains(searchName); // Filter by Lecturer.FullName
            }

            var lecturers = await _unitOfWork.GetRepository<CampusLecturer>().GetPagingListAsync(
                selector: x => new LecturerResponse
                {
                    Id = x.Lecturer.Id, // Get Lecturer Id
                    FullName = x.Lecturer.FullName, // Get Lecturer FullName
                    AccountId = x.Lecturer.AccountId,
                    State = x.Lecturer.State,
                    Status = x.Lecturer.Status,
                    DateCreated = x.Lecturer.DateCreated,
                    DateUpdated = x.Lecturer.DateUpdated
                    
                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Lecturer).ThenInclude(w => w.Wallets), // Include Lecturer and Wallets
                page: page,
                size: size);

            return lecturers;
        }

        protected string GetCampusIdFromJwt()
        {
            var id = _httpContextAccessor?.HttpContext?.User?.FindFirst("campusId");
            return id?.Value ?? string.Empty;
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

        public async  Task<LecturerResponse> CreateCampusLecture(List<string> campusIds, CreateLecturerModel lecturer, string accountId)
        {
            // 1. Validate input
            if (lecturer == null)
            {
                throw new ArgumentNullException(nameof(lecturer), "Lecturer model cannot be null.");
            }

            if (campusIds == null || !campusIds.Any())
            {
                throw new ArgumentException("At least one campus ID must be provided.", nameof(campusIds));
            }

            // Validate CreateLecturerModel
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(lecturer);
            if (!Validator.TryValidateObject(lecturer, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                throw new ArgumentException($"Validation failed: {errors}");
            }

            // 2. Kiểm tra xem các campusId có tồn tại không
            foreach (var campusId in campusIds)
            {
                var campusExists = await _unitOfWork.GetRepository<Campus>()
                    .AnyAsync(c => c.Id == campusId);

                if (!campusExists)
                {
                    throw new ArgumentException($"Campus with ID {campusId} does not exist.");
                }
            }

            // 3. Tạo Lecturer entity từ CreateLecturerModel
            var lecturerEntity = new Lecturer
            {
                Id = Ulid.NewUlid().ToString(), // Tạo ID mới cho Lecturer
                AccountId = accountId,
                FullName = lecturer.FullName,
                DateCreated = lecturer.DateCreated ?? DateTime.UtcNow, // Gán thời gian hiện tại nếu null
                DateUpdated = lecturer.DateUpdated,
                State = true,
                Status = lecturer.Status
            };

            // 4. Tạo liên kết với các campus
            lecturerEntity.CampusLecturers = campusIds.Select(campusId => new CampusLecturer
            {
                Id = Ulid.NewUlid().ToString(), // Tạo ID mới cho CampusLecturer
                LecturerId = lecturerEntity.Id,
                CampusId = campusId,
                Status = true // Gán mặc định Status là true, có thể thay đổi theo yêu cầu
            }).ToList();

            // 5. Lưu Lecturer vào database
            await _unitOfWork.GetRepository<Lecturer>().InsertAsync(lecturerEntity);
            await _unitOfWork.CommitAsync();

            // 6. Tạo LecturerResponse để trả về
            var lecturerResponse = new LecturerResponse
            {
                Id = lecturerEntity.Id,
                AccountId = lecturerEntity.AccountId,
                FullName = lecturerEntity.FullName,
                DateCreated = lecturerEntity.DateCreated,
                DateUpdated = lecturerEntity.DateUpdated,
                State = lecturerEntity.State,
                Status = lecturerEntity.Status,
                //CampusIds = campusIds // Trả về danh sách campusId đã liên kết
            };

            return lecturerResponse;
        }
    }
    
}
