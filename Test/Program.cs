using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SWallet.Repository.Services;
using SWallet.Repository.Services.Implements;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Tests
{
    [TestFixture]
    public class CloudinaryServiceTest
    {
        [Test]
        public async Task UploadFileAsync_ShouldUploadFileSuccessfully()
        {
            // Arrange
            var mockCloudinary = new Mock<Cloudinary>();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test content")));
            var uploadParams = new RawUploadParams { File = new FileDescription("test.jpg", fileMock.Object.OpenReadStream()) };

            mockCloudinary.Setup(c => c.UploadAsync(It.Is<RawUploadParams>(p => p.File.FileName == "test.jpg"), false, null, null))
                .ReturnsAsync(new RawUploadResult { SecureUrl = new Uri("https://example.com/image.jpg") });

            var service = new CloudinaryService(mockCloudinary.Object);

            // Act
            var result = await service.UploadFileAsync(fileMock.Object, "test-folder");

            // Assert
            Assert.AreEqual("https://example.com/image.jpg", result);
        }

        // ... các test case khác ...
    }
}