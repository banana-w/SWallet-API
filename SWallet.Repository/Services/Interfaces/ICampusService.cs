﻿using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campus;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campus;
using SWallet.Repository.Payload.Response.QRCodeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICampusService
    {
        Task<IPaginate<CampusResponse>> GetCampus(string searchName, int page, int size);
        Task<CampusResponse> GetCampusById(string id);
        Task<CampusResponse> CreateCampus(CreateCampusModel campus);
        Task<CampusResponse> UpdateCampus(string id, UpdateCampusModel campus);
        Task<CampusResponse> CreateCampusByAccountId(string accountId, CreateCampusByAccIdModel campus);
        Task<IPaginate<CampusResponse>> GetCampusByLectureId(string lectureId, string searchName, int page, int size);

        Task<CampusResponse> GetCampusByAccountId(string accountId);
        Task<bool> UpdateCampusStatus(string accountId, bool status);
        
        void Delete(string id);

    }
}
