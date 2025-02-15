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

        public AccountServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<SwalletDbContext>>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<AccountService>>();
            _mapperMock = new Mock<IMapper>();

            _accountService = new AccountService(
                _unitOfWorkMock.Object, _loggerMock.Object,
                _emailServiceMock.Object,
                _cloudinaryServiceMock.Object
                );
        }

        [Fact]
        public async Task CreateStudentAccount_ShouldReturnAccountResponse_WhenSuccess()
        {
            // Arrange
            var accountCreation = new CreateStudentAccount
            {
                UserName = "testuser",
                Password = "Test@1234",
                PasswordConfirmed = "Test@1234",
                MajorId = "CS",
                CampusId = "Main",
                FullName = "Test User",
                StudentCardFront = Mock.Of<IFormFile>(),
                StudentCardBack = Mock.Of<IFormFile>(),
                Code = "123456",
                Gender = 1,
                Email = "testuser@example.com",
                DateOfBirth = new DateOnly(2000, 1, 1),
                Phone = "1234567890",
                Address = "123 Test St",
                Description = "Test description",
                State = true
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
            var result = await _accountService.CreateStudentAccount(accountCreation);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().BeEquivalentTo("testuser@example.com");
            _emailServiceMock.Verify(e => e.SendEmailStudentRegister("testuser@example.com"), Times.Once);
        }
        [Fact]
        public async Task CreateStudentAccount_ShouldThrowException_WhenCommitFails()
        {
            // Arrange
            var accountCreation = new CreateStudentAccount
            {
                UserName = "testuser",
                Password = "Test@1234",
                PasswordConfirmed = "Test@1234",
                MajorId = "CS",
                CampusId = "Main",
                FullName = "Test User",
                StudentCardFront = Mock.Of<IFormFile>(),
                StudentCardBack = Mock.Of<IFormFile>(),
                Code = "123456",
                Gender = 1,
                Email = "testuser@example.com",
                DateOfBirth = new DateOnly(2000, 1, 1),
                Phone = "1234567890",
                Address = "123 Test St",
                Description = "Test description",
                State = true
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
                await _accountService.CreateStudentAccount(accountCreation)
            );

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailStudentRegister(It.IsAny<string>()), Times.Never);
        }


    }
}
