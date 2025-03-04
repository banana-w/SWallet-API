using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Services.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;



namespace SWallet.Repository.Services.Implements
{
    public class RedisService : BaseService<RedisService>, IRedisService
    {
        private readonly IDatabase _database;
        public RedisService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<RedisService> logger, IConnectionMultiplexer redis) : base(unitOfWork, logger)
        {
            _database = redis.GetDatabase();
        }

        public async Task SaveVerificationCodeAsync(string email, string code)
        {
            await _database.StringSetAsync($"verify:{email}", code, TimeSpan.FromMinutes(10));
        }

        public async Task<bool> VerifyCodeAsync(string email, string userInput)
        {
            var storedCode = await _database.StringGetAsync($"verify:{email}");
            if (string.IsNullOrEmpty(storedCode)) return false;

            if (BCryptNet.Verify(userInput, storedCode))
            {
                await _database.KeyDeleteAsync($"verify:{email}"); // Xóa mã sau khi xác thực
                return true;
            }
            return false;
        }
    }
}
