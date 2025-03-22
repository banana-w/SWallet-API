using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Payload.Response.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ILecturerService
    {
        Task<IPaginate<LecturerResponse>> GetLecturers(string searchName, int page, int size);
        Task<LecturerResponse> GetLecturerById(string id);
        Task<LecturerResponse> CreateLecturerAccount(CreateLecturerModel lecturer);
        Task<LecturerResponse> UpdateLecturer(string id, UpdateLecturerModel lecturer);

        Task<IPaginate<LecturerResponse>> GetLecturersByCampusId(string campusId, string searchName, int page, int size);
        void Delete(string id);

    }
}
