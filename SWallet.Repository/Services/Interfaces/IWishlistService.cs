
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.WishList;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Payload.Response.Wishlist;

namespace SWallet.Repository.Services.Interfaces
{

    public interface IWishlistService
    {

        Task<IPaginate<WishlListModel>>  GetAll
            (List<string> studentIds, List<string> brandIds, string? search, int page, int size);

        Task<WishlListModel> UpdateWishlist(WishListUpdateModel update);
        Task<List<string>> GetWishlishBrandIdByStudentId(string studentId);
        Task<List<string>> GetUnWishlishBrandIdByStudentId(string studentId);
    }
}