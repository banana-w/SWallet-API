using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;

namespace SWallet.Repository.Services
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<SwalletDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<T> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public BaseService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<T> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected string GetUsernameFromJwt()
        {
            var nameClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return nameClaim?.Value ?? string.Empty;
        }

        protected string GetRoleFromJwt()
        {
            var roleClaim = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? string.Empty;
        }

        //Use for employee and store manager
        //protected async Task<bool> CheckIsUserInStore(Account account, Store store)
        //{
        //    ICollection<StoreAccount> storeAccount = await _unitOfWork.GetRepository<StoreAccount>()
        //        .GetListAsync(predicate: s => s.StoreId.Equals(store.Id));
        //    return storeAccount.Select(x => x.AccountId).Contains(account.Id);
        //}

        protected string GetBrandIdFromJwt()
        {
            var id = _httpContextAccessor?.HttpContext?.User?.FindFirst("brandId");
            return id?.Value ?? string.Empty;
        }
        //protected string GetOrganizationIdFromJwt()
        //{
        //    return _httpContextAccessor?.HttpContext?.User?.FindFirstValue("organizationId");
        //}
        //protected string GetStoreIdFromJwt()
        //{
        //    return _httpContextAccessor?.HttpContext?.User?.FindFirstValue("storeId");
        //}
    }
}