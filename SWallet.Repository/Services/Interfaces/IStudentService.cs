using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponse> CreateStudentAsync(string accountId, StudentRequest studentRequest);
        Task<StudentResponse> UpdateStudentAsync(string accountId, StudentRequest studentRequest);
        Task<IPaginate<StudentResponse>> GetStudentsAsync(string search, bool? isAsc, int pageIndex, int pageSize);
        Task<StudentResponse> GetStudentAsync(string studentId);
        Task<StudentResponse> GetStudentByAccountIdAsync(string accountId);
        Task<bool> ValidEmailStudent(string email);

        //Task<bool> DeleteStudentAsync(string accountId, string studentId);
        //Task<bool> VerifyStudentAsync(string accountId, string studentId);
        //Task<bool> UnVerifyStudentAsync(string accountId, string studentId);
        //Task<bool> LockStudentAsync(string accountId, string studentId);
        //Task<bool> UnLockStudentAsync(string accountId, string studentId);
    }
}
