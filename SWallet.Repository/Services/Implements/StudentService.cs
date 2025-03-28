using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Student;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;

namespace SWallet.Repository.Services.Implements
{
    public class StudentService : BaseService<StudentService>, IStudentService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public StudentService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<StudentService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }
        public async Task<StudentResponse> CreateStudentAsync(string accountId, StudentRequest studentRequest)
        {
            if (string.IsNullOrEmpty(accountId) || studentRequest == null)
            {
                throw new ArgumentException("Invalid accountId or studentRequest");
            }
            var imageUri = string.Empty;
            if (studentRequest.StudentCardFront != null && studentRequest.StudentCardFront.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(studentRequest.StudentCardFront);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var student = new Student
            {
                Id = Ulid.NewUlid().ToString(),
                CampusId = studentRequest.CampusId,
                AccountId = accountId,
                StudentCardFront = imageUri,
                StudentCardBack = null,
                Address = studentRequest.Address,
                DateOfBirth = studentRequest.DateOfBirth,
                Code = studentRequest.Code,
                FullName = studentRequest.FullName,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Gender = studentRequest.Gender,
                State = (int?)StudentState.Pending,
                Status = true,
                TotalIncome = 0,
                TotalSpending = 0,

            };

            await _unitOfWork.GetRepository<Student>().InsertAsync(student);
            var result = await _unitOfWork.CommitAsync();

            if (result > 0)
            {
                return new StudentResponse
                {
                    Id = student.Id,
                    CampusId = student.CampusId,
                    AccountId = student.AccountId,
                    StudentEmail = student.StudentEmail,
                    StudentCardFront = student.StudentCardFront,
                    StudentCardBack = student.StudentCardBack,
                    Address = student.Address,
                    DateOfBirth = student.DateOfBirth,
                    Code = student.Code,
                    FullName = student.FullName,
                    DateCreated = student.DateCreated,
                    DateUpdated = student.DateUpdated,
                    Gender = student.Gender,
                    State = student.State,
                    Status = student.Status,
                    TotalIncome = student.TotalIncome,
                    TotalSpending = student.TotalSpending,
                    
                };
            }
            throw new ApiException("Create student fail", 400, "STUDENT_FAIL");
        }

        public async Task<StudentResponse> GetStudentAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                throw new ArgumentException("Invalid studentId");
            }

            var student = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(
                selector: x => new StudentResponse
                {
                    Id = x.Id,
                    CampusId = x.CampusId,
                    AccountId = x.AccountId,
                    StudentCardFront = x.StudentCardFront,
                    StudentCardBack = x.StudentCardBack,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    StudentEmail = x.StudentEmail,
                    Code = x.Code,
                    FullName = x.FullName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Gender = x.Gender,
                    State = x.State,
                    Status = x.Status,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,

                },
                    predicate: x => x.Id == studentId);
            if (student == null)
            {
                return null;
            }
            return student;
        }

        public async Task<StudentResponse> GetStudentByAccountIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Invalid Id");
            }

            var student = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(
                selector: x => new StudentResponse
                {
                    Id = x.Id,
                    CampusId = x.CampusId,
                    AccountId = x.AccountId,
                    StudentCardFront = x.StudentCardFront,
                    StudentCardBack = x.StudentCardBack,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Code = x.Code,
                    FullName = x.FullName,
                    StudentEmail = x.StudentEmail,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Gender = x.Gender,
                    State = x.State,
                    Status = x.Status,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,

                },
                    predicate: x => x.AccountId == accountId);
            if (student == null)
            {
                return null;
            }
            return student;

        }

        public async Task<IPaginate<StudentResponse>> GetStudentsAsync(string search, bool? isAsc, int pageIndex, int pageSize)
        {
            Expression<Func<Student, bool>> filterQuery = x =>
                string.IsNullOrEmpty(search) || x.FullName.Contains(search) || x.Code.Contains(search);

            var students = await _unitOfWork.GetRepository<Student>().GetPagingListAsync(
                selector: x => new StudentResponse
                {
                    Id = x.Id,
                    CampusId = x.CampusId,
                    AccountId = x.AccountId,
                    StudentCardFront = x.StudentCardFront,
                    StudentCardBack = x.StudentCardBack,
                    StudentEmail = x.StudentEmail,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Code = x.Code,
                    FullName = x.FullName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Gender = x.Gender,
                    State = x.State,
                    Status = x.Status,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                },
                predicate: filterQuery,
                page: pageIndex,
                size: pageSize,
                orderBy: x => isAsc.HasValue && isAsc.Value ? x.OrderBy(v => v.DateCreated) : x.OrderByDescending(v => v.DateCreated)
                );

            return students;
        }

        public async Task<StudentResponse> UpdateStudentAsync(string accountId, StudentRequest studentRequest)
        {
            if (string.IsNullOrEmpty(accountId) || studentRequest == null)
            {
                throw new ArgumentException("Invalid accountId or studentRequest");
            }

            var student = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId);
            if (student == null)
            {
                 throw new ApiException(" student null", 400, "STUDENT_FAIL"); ;
            }

            var imageUri = student.StudentCardFront;
            if (studentRequest.StudentCardFront != null && studentRequest.StudentCardFront.Length > 0)
            {
                if (!string.IsNullOrEmpty(student.StudentCardFront))
                {
                    await _cloudinaryService.RemoveImageAsync(student.StudentCardFront);
                }
                var uploadResult = await _cloudinaryService.UploadImageAsync(studentRequest.StudentCardFront);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            student.CampusId = studentRequest.CampusId;
            student.StudentCardFront = imageUri;
            student.Address = studentRequest.Address;
            student.DateOfBirth = studentRequest.DateOfBirth;
            student.Code = studentRequest.Code;
            student.FullName = studentRequest.FullName;
            student.DateUpdated = DateTime.Now;
            student.Gender = studentRequest.Gender;
            student.State = (int?)StudentState.Active;

            _unitOfWork.GetRepository<Student>().UpdateAsync(student);
            var result = await _unitOfWork.CommitAsync();

            if (result > 0)
            {
                return new StudentResponse
                {
                    Id = student.Id,
                    CampusId = student.CampusId,
                    AccountId = student.AccountId,
                    StudentCardFront = student.StudentCardFront,
                    StudentCardBack = student.StudentCardBack,
                    Address = student.Address,
                    DateOfBirth = student.DateOfBirth,
                    Gender = student.Gender,
                    Code = student.Code,
                    FullName = student.FullName,
                    DateCreated = student.DateCreated,
                    DateUpdated = student.DateUpdated,
                };
            }
            throw new ApiException("Update student fail", 400, "STUDENT_FAIL");
        }

        public async Task<StudentResponse> UpdateStudentCardFront(string studentId, IFormFile studentCardFront)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                throw new ArgumentException("Invalid Id");
            }
            var student = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: x => x.Id == studentId);
            if (student == null)
            {
                throw new ApiException("Student not found", 400, "STUDENT_NOT_FOUND");
            }

            var imageUri = string.Empty;
            if (studentCardFront != null && studentCardFront.Length > 0)
            {
                if (!string.IsNullOrEmpty(student.StudentCardFront))
                {
                    await _cloudinaryService.RemoveImageAsync(student.StudentCardFront);
                }
                var uploadResult = await _cloudinaryService.UploadImageAsync(studentCardFront);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }
            else
            {
                throw new ApiException("StudentCardFront is null", 400, "STUDENT_FAIL");
            }
            student.StudentCardFront = imageUri;
            student.DateUpdated = DateTime.Now;

            _unitOfWork.GetRepository<Student>().UpdateAsync(student);
            var result = await _unitOfWork.CommitAsync();

            if (result > 0)
            {
                return new StudentResponse
                {
                    Id = student.Id,
                    CampusId = student.CampusId,
                    AccountId = student.AccountId,
                    StudentCardFront = student.StudentCardFront,
                    StudentCardBack = student.StudentCardBack,
                    Address = student.Address,
                    DateOfBirth = student.DateOfBirth,
                    Gender = student.Gender,
                    Code = student.Code,
                    FullName = student.FullName,
                    DateCreated = student.DateCreated,
                    DateUpdated = student.DateUpdated,
                };
            }
            throw new ApiException("Update student fail", 400, "STUDENT_FAIL");
        }

        public Task<bool> ValidEmailStudent(string email)
        {
            var result = _unitOfWork.GetRepository<Student>().AnyAsync(x => x.StudentEmail == email);
            if (result.Result)
            {
                throw new ApiException("Email already exists", 400, "BAD_REQUEST");
            }
            return Task.FromResult(true);
        }
    }
}
