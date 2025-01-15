using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using SWallet.Repository.Services.Implements;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SWallet.Tests.Services
{
    public class CloudinaryServiceTests
    {
        [Fact]
        public async Task UploadFileAsync_ThrowsArgumentException_ForNullFile()
        {
            var cloudinaryMock = new Mock<Cloudinary>();
            var service = new CloudinaryService(cloudinaryMock.Object);
            IFormFile fileUpload = null;
            string folder = "test-folder";

            await Assert.ThrowsAsync<ArgumentException>(() => service.UploadFileAsync(fileUpload, folder));
        }

        [Fact]
        public async Task UploadFileAsync_ThrowsArgumentException_ForEmptyFile()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);
            var fileUpload = mockFile.Object;
            string folder = "test-folder";
            var cloudinaryMock = new Mock<Cloudinary>();
            var service = new CloudinaryService(cloudinaryMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.UploadFileAsync(fileUpload, folder));
        }

        [Fact]
        public async Task UploadFileAsync_CallsCloudinaryUploadAsync_WithCorrectParams()
        {
            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes("test")); // Use MemoryStream
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.OpenReadStream()).Returns(mockStream);
            var fileUpload = mockFile.Object;
            string folder = "test-folder";

            var cloudinaryMock = new Mock<Cloudinary>();
            var expectedResult = new RawUploadResult { SecureUrl = new Uri("http://test.com/image.jpg") }; // Correct usage
            cloudinaryMock.Setup(c => c.UploadAsync(It.IsAny<RawUploadParams>()))
                .ReturnsAsync(expectedResult);

            var service = new CloudinaryService(cloudinaryMock.Object);

            var result = await service.UploadFileAsync(fileUpload, folder);

            Assert.Equal(expectedResult.SecureUrl.AbsoluteUri, result); // Assert the URL
        }

        [Fact]
        public async Task UploadFileAsync_ThrowsException_ForCloudinaryError()
        {
            var mockStream = new MemoryStream();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.OpenReadStream()).Returns(mockStream);
            var fileUpload = mockFile.Object;
            string folder = "test-folder";

            var cloudinaryMock = new Mock<Cloudinary>();
            var expectedError = new Error { Message = "Cloudinary error" };
            var uploadResult = new RawUploadResult { Error = expectedError }; // Correct usage
            cloudinaryMock.Setup(c => c.UploadAsync(It.IsAny<RawUploadParams>()))
                .ReturnsAsync(uploadResult);

            var service = new CloudinaryService(cloudinaryMock.Object);

            await Assert.ThrowsAsync<Exception>(() => service.UploadFileAsync(fileUpload, folder));
        }

        // Add similar tests for DeleteFileAsync
    }
}