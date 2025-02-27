using AutoMapper;
using CloudinaryDotNet.Actions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Swallet_UnitTest.Services
{
    public class AccountServiceTest
    {
        private readonly Mock<IUnitOfWork<SwalletDbContext>> _unitOfWorkMock;
        private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly AccountService _accountService;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBrandService> _brandServiceMock;
        private readonly Mock<IStudentService> _studentServiceMock;
        public AccountServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<SwalletDbContext>>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<AccountService>>();
            _mapperMock = new Mock<IMapper>();
            _brandServiceMock = new Mock<IBrandService>();
            _studentServiceMock = new Mock<IStudentService>();

            _accountService = new AccountService(
                _unitOfWorkMock.Object, _loggerMock.Object,
                _emailServiceMock.Object,
                _cloudinaryServiceMock.Object,
                _brandServiceMock.Object,
                _studentServiceMock.Object

                );
        }

        [Fact]
        public async Task CreateStudentAccount_ShouldReturnAccountResponse_WhenSuccess()
        {
            // Arrange
            var accountRequest = new AccountRequest
            {
                UserName = "testuser",
                Password = "Test@1234",
                Phone = "1234567890",
                Email = "testuser@example.com",
            };
            var studentRequest = new StudentRequest
            {
                CampusId = "Main",
                FullName = "Test User",
                StudentCardFront = Mock.Of<IFormFile>(),
                Code = "123456",
                AccountId = "1",
                Address = "123 Test St",
                DateOfBirth = new DateOnly(2000, 1, 1),
                Gender = 1,
            };

            var account = new Account { Id = "1", Email = "student1@example.com" };
            var student = new Student { AccountId = "1" };
            var accountResponse = new AccountResponse { Id = "1", Email = "student1@example.com" };

            
            _cloudinaryServiceMock.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), null, null)).ReturnsAsync(new ImageUploadResult
            {
                SecureUrl = new Uri("http://example.com/image.jpg"),
                PublicId = "image123"
            });
            _unitOfWorkMock.Setup(u => u.GetRepository<Account>().InsertAsync(account)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.GetRepository<Student>().InsertAsync(student)).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(e => e.SendEmailStudentRegister(It.IsAny<string>()));
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _accountService.CreateStudentAccount(accountRequest, studentRequest);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().BeEquivalentTo("testuser@example.com");
            _emailServiceMock.Verify(e => e.SendEmailStudentRegister("testuser@example.com"), Times.Once);
        }
        [Fact]
        public async Task CreateStudentAccount_ShouldThrowException_WhenCommitFails()
        {
            // Arrange
            var accountRequest = new AccountRequest
            {
                UserName = "testuser",
                Password = "Test@1234",
                Phone = "1234567890",
                Email = "testuser@example.com",
            };
            var studentRequest = new StudentRequest
            {
                CampusId = "Main",
                FullName = "Test User",
                StudentCardFront = Mock.Of<IFormFile>(),
                Code = "123456",
                AccountId = "1",
                Address = "123 Test St",
                DateOfBirth = new DateOnly(2000, 1, 1),
                Gender = 1,
            };
            var account = new Account { Id = "1", Email = "student1@example.com" };
            var student = new Student { AccountId = "1" };

            _cloudinaryServiceMock.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), null, null)).ReturnsAsync(new ImageUploadResult
            {
                SecureUrl = new Uri("http://example.com/image.jpg"),
                PublicId = "image123"
            });
            _unitOfWorkMock.Setup(u => u.GetRepository<Account>().InsertAsync(account)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.GetRepository<Student>().InsertAsync(student)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0); // Commit thất bại

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(async () =>
                await _accountService.CreateStudentAccount(accountRequest, studentRequest)
            );

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailStudentRegister(It.IsAny<string>()), Times.Never);
        }


    }
}
