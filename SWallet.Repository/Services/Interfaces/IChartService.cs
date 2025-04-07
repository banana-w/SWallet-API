using SWallet.Repository.Payload.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IChartService
    {
        Task<List<ColumnChartModel>> GetColumnChart
        (string id, DateOnly fromDate, DateOnly toDate, bool? isAsc, string role);

        Task<List<LineChartModel>> GetLineChart(string id, string role);

        Task<List<RankingModel>> GetRankingChart(string id, Type type, string role);

        Task<TitleAdminModel> GetTitleAdmin(string adminId);

        Task<TitleBrandModel> GetTitleBrand(string brandId);

        Task<TitleStoreModel> GetTitleStore(string storeId);
    }
}
