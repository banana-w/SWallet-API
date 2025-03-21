using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.Authentication;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;


namespace Swallet_UnitTest.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<IUnitOfWork<SwalletDbContext>> _unitOfWorkMock;
        private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthenticationService _authService;
        private readonly Mock<IRedisService> _redisServiceMock;

        public AuthServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<SwalletDbContext>>();
            _loggerMock = new Mock<ILogger<AuthenticationService>>();
            _jwtServiceMock = new Mock<IJwtService>();
            _mapperMock = new Mock<IMapper>();
            _redisServiceMock = new Mock<IRedisService>();
            _authService = new AuthenticationService(_unitOfWorkMock.Object, _loggerMock.Object, _jwtServiceMock.Object, _redisServiceMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "JohnDoe", Password = "password123" };
            var account = new Account { UserName = "JohnDoe", Password = BCrypt.Net.BCrypt.HashPassword("password123"), Role = 1, Id = "1" };
            var accountResponse = new AccountResponse { UserId = "1", RoleName = "Admin" };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Account>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(),
                    It.IsAny<Func<IQueryable<Account>, IIncludableQueryable<Account, object>>>()))
                .ReturnsAsync(account);
            _mapperMock.Setup(m => m.Map<AccountResponse>(account)).Returns(accountResponse);

            _jwtServiceMock.Setup(j => j.GenerateJwtToken(It.IsAny<AccountResponse>(), It.IsAny<Tuple<string, string>>())).Returns("token");

            // Act
            var result = await _authService.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Quản trị viên", result.Role);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }


    }
}
