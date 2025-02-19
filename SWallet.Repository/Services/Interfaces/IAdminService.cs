using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWallet.Repository.Payload.Response.Admin;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminResponse> Add(CreateAdminModel creation);

        Task<IPaginate<AdminResponse>> GetAll(string searchName, int page, int size);

        Task<AdminResponse> GetById(string id);

        Task<AdminResponse> Update(string id, UpdateAdminModel update);
    }
}
