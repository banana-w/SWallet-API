﻿using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Payload.Response.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<IPaginate<CampaignResponse>> GetCampaignsInBrand(string? searchName, int page, int size);
        Task<IPaginate<CampaignResponse>> GetCampaignsByBrandId(string brandId,string? searchName, int page, int size);
        Task<IPaginate<CampaignResponse>> GetCampaignsByBrandIdMobile(string brandId,string? searchName, int page, int size);
        Task<IPaginate<CampaignResponseAllStatus>> GetCampaignsByBrandIdAllStatus(string brandId,string? searchName, int page, int size);
        Task<IPaginate<CampaignResponse>> GetCampaigns(string? searchName, int page, int size);
        Task<IPaginate<CampaignResponseAllStatus>> GetCampaignsAll(string? searchName, int page, int size);
        Task<CampaignResponseExtra> GetCampaignById(string id);
        Task<CampaignResponseExtraAllStatus> GetCampaignByIdAllStatus(string id);
        Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaign, List<CreateCampaignDetailModel> campaignDetails);
        Task<CampaignResponse> ApproveOrRejectCampaign(string campaignId, bool isApproved, string? rejectionReason = null);
        Task<CampaignResponse> UpdateCampaign(string id, UpdateCampaignModel campaign);

        Task<IPaginate<StoreResponse>> GetStoresByCampaignId(string campaignId, string searchName, int page, int size);

        Task<IEnumerable<CampaignDetailResponse>> GetAllCampaignDetails();

        Task<List<CampaignResponse>> GetRanking(string brandId, int limit);
        Task<List<CampaignRankingResponse>> GetRankingByVoucherRatio(string brandId, int limit);

        void Delete(string id);
        Task<int> CountCampaign();

    }
}
