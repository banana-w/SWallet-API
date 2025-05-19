using AutoMapper;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campus;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campus;
using SWallet.Repository.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using SWallet.Repository.Payload.Response.QRCodeResponse;

namespace SWallet.Repository.Services.Implements
{
    public class CampusService : BaseService<CampusService>, ICampusService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public CampusService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampusService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Campus, CreateCampusModel>()
            .ReverseMap();
            });
            mapper = new Mapper(config);
        }

        public async Task<CampusResponse> CreateCampus(CreateCampusModel campus)
        {

            var existingArea = await _unitOfWork.GetRepository<Area>().SingleOrDefaultAsync(
               predicate: b => b.Id == campus.AreaId);

            if (existingArea == null)
            {
                throw new ApiException("Area not found", 400, "BAD_REQUEST");
            }

            var imageUri = string.Empty;
            if (campus.Image != null && campus.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(campus.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var newCampus = new Campus
            {
                Id = Ulid.NewUlid().ToString(),
                AreaId = campus.AreaId,
                CampusName = campus.CampusName,
                Address = campus.Address,
                Phone = campus.Phone,
                Email = campus.Email,
                LinkWebsite = campus.Link,
                Image = imageUri,
                FileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = campus.Description,
                State = campus.State,
                Status = true
            };
            await _unitOfWork.GetRepository<Campus>().InsertAsync(newCampus);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new CampusResponse
                {
                    Id = newCampus.Id,
                    AreaId = newCampus.AreaId,
                    AreaName = existingArea.AreaName,
                    CampusName = newCampus.CampusName,
                    Address = newCampus.Address,
                    Phone = newCampus.Phone,
                    Email = newCampus.Email,
                    Link = newCampus.LinkWebsite,
                    Image = newCampus.Image,
                    FileName = newCampus.FileName,
                    DateCreated = newCampus.DateCreated,
                    DateUpdated = newCampus.DateUpdated,
                    Description = newCampus.Description,
                    State = newCampus.State,
                    Status = newCampus.Status   

                };
            }
            throw new ApiException("Create Campus Fail", 400, "BAD_REQUEST");
        }

        public async Task<CampusResponse> CreateCampusByAccountId(string accountId, CreateCampusByAccIdModel campus)
        {
            {
                var existingArea = await _unitOfWork.GetRepository<Area>().SingleOrDefaultAsync(
               predicate: b => b.Id == campus.AreaId);

                if (existingArea == null)
                {
                    throw new ApiException("Area not found", 400, "BAD_REQUEST");
                }

                var imageUri = string.Empty;
                if (campus.Image != null && campus.Image.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(campus.Image);
                    imageUri = uploadResult.SecureUrl.AbsoluteUri;
                }

                var newCampus = new Campus
                {
                    Id = Ulid.NewUlid().ToString(),
                    AreaId = campus.AreaId,
                    CampusName = campus.CampusName,
                    Address = campus.Address,
                    Phone = campus.Phone,
                    Email = campus.Email,
                    LinkWebsite = campus.Link,
                    Image = imageUri,
                    FileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Description = campus.Description,
                    State = campus.State,
                    Status = true,
                    AccountId = accountId // Use the provided accountId
                };

                await _unitOfWork.GetRepository<Campus>().InsertAsync(newCampus);
                var isSuccess = await _unitOfWork.CommitAsync() > 0;

                if (isSuccess)
                {
                    return new CampusResponse
                    {
                        Id = newCampus.Id,
                        AreaId = newCampus.AreaId,
                        AreaName = existingArea.AreaName,
                        CampusName = newCampus.CampusName,
                        Address = newCampus.Address,
                        Phone = newCampus.Phone,
                        Email = newCampus.Email,
                        Link = newCampus.LinkWebsite,
                        Image = newCampus.Image,
                        FileName = newCampus.FileName,
                        DateCreated = newCampus.DateCreated,
                        DateUpdated = newCampus.DateUpdated,
                        Description = newCampus.Description,
                        State = newCampus.State,
                        Status = newCampus.Status,
                        AccountId = newCampus.AccountId
                    };
                }

                throw new ApiException("Create Brand Fail", 400, "BAD_REQUEST");
            }
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<CampusResponse>> GetCampus(string searchName, int page, int size)
        {
            Expression<Func<Campus, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.CampusName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Campus>().GetPagingListAsync(
                selector: x => new CampusResponse
                {
                    Id = x.Id,
                    AreaId = x.AreaId,
                    AccountId = x.AccountId,
                    AreaName = x.Area.AreaName,
                    CampusName = x.CampusName,
                    Address = x.Address,
                    Phone = x.Phone,
                    Email = x.Email,
                    Link = x.LinkWebsite,
                    Image = x.Image,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status

                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<CampusResponse> GetCampusByAccountId(string accountId)
        {
            var area = await _unitOfWork.GetRepository<Campus>().SingleOrDefaultAsync(
                selector: x => new CampusResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    CampusName = x.CampusName,
                    Address = x.Address,
                    Phone = x.Phone,
                    Email = x.Email,
                    Link = x.LinkWebsite,
                    Image = x.Image,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfStudents = x.Students.Count
                },
                predicate: x => x.AccountId == accountId); 
            return area;
        }

        public async Task<CampusResponse> GetCampusById(string id)
        {
            var area = await _unitOfWork.GetRepository<Campus>().SingleOrDefaultAsync(
                selector: x => new CampusResponse
                {
                    Id = x.Id,
                    AreaId = x.AreaId,
                    AccountId = x.AccountId,
                    AreaName = x.Area.AreaName,
                    CampusName = x.CampusName,
                    Address = x.Address,
                    Phone = x.Phone,
                    Email = x.Email,
                    Link = x.LinkWebsite,
                    Image = x.Image,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfStudents = x.Students.Count
                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<IPaginate<CampusResponse>> GetCampusByLectureId(string lectureId, string searchName, int page, int size)
        {
            if (string.IsNullOrEmpty(lectureId))
            {
                throw new ApiException("LectureId cannot be empty", 400, "BAD_REQUEST");
            }
            Expression<Func<CampusLecturer, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.LecturerId == lectureId;
            }
            else
            {
                filterQuery = p => p.LecturerId == lectureId && p.LecturerId.Contains(searchName);
            }

            var history = await _unitOfWork.GetRepository<CampusLecturer>().GetPagingListAsync(
                selector: x => new CampusResponse
                {
                    Id = x.Id,
                    AreaId = x.Campus.AreaId,
                    AreaName = x.Campus.Area.AreaName,
                    CampusName = x.Campus.CampusName,
                    Address = x.Campus.Address,
                    Phone = x.Campus.Phone,
                    Email = x.Campus.Email,
                    Link = x.Campus.LinkWebsite,
                    Image = x.Campus.Image,
                    DateCreated = x.Campus.DateCreated,
                    DateUpdated = x.Campus.DateUpdated,
                    Description = x.Campus.Description,
                    State = x.Campus.State,
                    Status = x.Campus.Status,

                 

                },
                predicate: filterQuery,
                include: query => query.Include(x => x.Campus).ThenInclude(x => x.Area),

                page: page,
                size: size);

            return history;
        }

        public async Task<CampusResponse> UpdateCampus(string id, UpdateCampusModel campus)
        {
            var updateCampus = await _unitOfWork.GetRepository<Campus>()
        .SingleOrDefaultAsync(
            predicate: x => x.Id == id,
            include: query => query.Include(x => x.Area)
        );
            if (updateCampus == null)
            {
                throw new ApiException("Campus not found", 404, "NOT_FOUND");
            }
            if (campus.Image != null && campus.Image.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(campus.Image);

            }
            updateCampus.AreaId = campus.AreaId;
            updateCampus.CampusName = campus.CampusName;
            updateCampus.Address = campus.Address;
            updateCampus.Phone = campus.Phone;
            updateCampus.Email = campus.Email;
            updateCampus.LinkWebsite = campus.Link;
            updateCampus.Description = campus.Description;
            updateCampus.State = campus.State;
            updateCampus.DateUpdated = DateTime.Now;
             _unitOfWork.GetRepository<Campus>().UpdateAsync(updateCampus);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new CampusResponse
                {
                    Id = updateCampus.Id,
                    AreaId = updateCampus.AreaId,
                    AreaName = updateCampus.Area.AreaName,
                    CampusName = updateCampus.CampusName,
                    Address = updateCampus.Address,
                    Phone = updateCampus.Phone,
                    Email = updateCampus.Email,
                    Link = updateCampus.LinkWebsite,
                    Image = updateCampus.Image,
                    DateCreated = updateCampus.DateCreated,
                    DateUpdated = updateCampus.DateUpdated,
                    Description = updateCampus.Description,
                    State = updateCampus.State,
                    Status = updateCampus.Status,


                };
            }
            throw new ApiException("Update Brand Fail", 400, "BAD_REQUEST");

        }

        public async Task<bool> UpdateCampusStatus(string accountId, bool status)
        {
            var campus = await _unitOfWork.GetRepository<Campus>()
                .SingleOrDefaultAsync(predicate: x => x.Id == accountId);
            if (campus == null)
            {
                throw new ApiException("Campus not found", 404, "NOT_FOUND");
            }
            campus.Status = status;
            campus.DateUpdated = DateTime.Now;

            _unitOfWork.GetRepository<Campus>().UpdateAsync(campus);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return true;
            }
            return false;
        }
    }
}
