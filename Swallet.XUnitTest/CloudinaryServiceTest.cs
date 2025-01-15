using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using SWallet.Repository.Services.Implements;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Swallet.XUnitTest.CloudinaryServiceTest
{
    public class CloudinaryServiceTest
    {
        private readonly Mock<Cloudinary> _mockCloudinary;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ITestOutputHelper _output;

        public CloudinaryServiceTest(ITestOutputHelper output)
        {
            _mockCloudinary = new Mock<Cloudinary>();
            _cloudinaryService = new CloudinaryService(_mockCloudinary.Object);
            _output = output; // Inject ITestOutputHelper
        }

        [Fact]
        public async Task UploadFileAsync_ValidFile_ReturnsUrl()
        {
            // Arrange
            _output.WriteLine("Starting test: UploadFileAsync_ValidFile_ReturnsUrl");

            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var folder = "test-folder";
            var expectedUrl = "https://res.cloudinary.com/test/image/upload/v123/test-folder/test.jpg";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file"));
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri(expectedUrl)
            };

            _mockCloudinary
                .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            // Act
            _output.WriteLine("Calling UploadFileAsync...");
            var result = await _cloudinaryService.UploadFileAsync(fileMock.Object, folder);

            // Assert
            _output.WriteLine($"Expected URL: {expectedUrl}, Result URL: {result}");
            Assert.Equal(expectedUrl, result);
            _output.WriteLine("Test passed: UploadFileAsync_ValidFile_ReturnsUrl");
        }

        //[Fact]
        //public async Task UploadFileAsync_InvalidFile_ThrowsException()
        //{
        //    // Arrange
        //    _output.WriteLine("Starting test: UploadFileAsync_InvalidFile_ThrowsException");

        //    var fileMock = new Mock<IFormFile>();
        //    var folder = "test-folder";

        //    fileMock.Setup(f => f.Length).Returns(0);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentException>(
        //        () => _cloudinaryService.UploadFileAsync(fileMock.Object, folder)
        //    );

        //    _output.WriteLine("Test passed: UploadFileAsync_InvalidFile_ThrowsException");
        //}
    }
}
