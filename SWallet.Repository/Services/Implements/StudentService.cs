using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using static SWallet.Repository.Repository.IStudentRepository;

namespace SWallet.Repository.Services.Implements
{
    public class StudentService : BaseService<StudentService>, IStudentService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly SwalletDbContext _dbContext;
        public StudentService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<StudentService> logger, ICloudinaryService cloudinaryService, SwalletDbContext dbContext) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            _dbContext = dbContext;

        }

        public async Task<List<Student>> GetRanking(int limit)
        {
            if (limit <= 0)
            {
                throw new ArgumentException("Giới hạn phải lớn hơn 0", nameof(limit));
            }

            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách xếp hạng sinh viên với giới hạn {Limit}", limit);

                var result = await _dbContext.Students
                    .Where(s => (bool)s.Status)
                    .OrderByDescending(s => s.TotalSpending)
                    .Take(limit)
                    .Include(s => s.Account)
                    .ToListAsync();

                _logger.LogInformation("Lấy thành công {Count} sinh viên", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách xếp hạng sinh viên với giới hạn {Limit}", limit);
                throw new Exception($"Lỗi khi lấy danh sách xếp hạng sinh viên: {ex.Message}", ex);
            }
        }
        

        public async Task<long> CountStudentToday(DateOnly date)
    {
        if (date == default)
        {
            throw new ArgumentException("Ngày không được để trống", nameof(date));
        }

        try
        {
            return await _dbContext.Students
                .Where(c => (bool)c.Status && c.DateCreated.HasValue
                           && DateOnly.FromDateTime(c.DateCreated.Value).Equals(date))
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đếm số lượng sinh viên vào ngày {Date}", date);
            throw new Exception("Lỗi khi đếm số lượng sinh viên: " + ex.Message, ex);
        }
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
                    CampusName = x.Campus.CampusName,
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
                    predicate: x => x.Id == studentId,
                    include: x => x.Include(x => x.Campus));
            if (student == null)
            {
                throw new ApiException("Student not found", 400, "STUDENT_NOT_FOUND");
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
                    CampusName = x.Campus.CampusName,
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
                    CoinBalance = x.Wallets.FirstOrDefault(w => w.Type == 1).Balance ?? 0

                },
                    predicate: x => x.AccountId == accountId,
                    include: x => x.Include(c => c.Campus));
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
                    State = student.State,
                    Status = student.Status,
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

        public long CountStudent()
        {
            long count = 0;
            try
            {
                var db = _dbContext;
                count = db.Students.Where(c => (bool)c.Status).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }


        public async Task<List<StudentRanking>> GetRankingByStore(string storeId, int limit)
        {
            if (string.IsNullOrEmpty(storeId))
                throw new ArgumentException("StoreId không được để trống", nameof(storeId));
            if (limit <= 0)
                throw new ArgumentException("Giới hạn phải lớn hơn 0", nameof(limit));

            try
            {
                return await _dbContext.Students
                    .Where(s => (bool)s.Status) // Lọc theo Status của Student
                    .Include(s => s.Account)
                    .OrderByDescending(s => s.TotalSpending) // Sắp xếp theo TotalSpending
                    .Take(limit)
                    .Select(s => new StudentRanking
                    {
                        Name = s.FullName,
                        Image = s.Account.Avatar,
                        TotalSpending = s.TotalSpending
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách xếp hạng sinh viên theo cửa hàng: {ex.Message}", ex);
            }
        }
    }
}
