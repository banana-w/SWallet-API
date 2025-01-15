using Moq;
using CloudinaryDotNet;
using SWallet.Repository.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using SWallet.Repository.Services.Implements;

public class CloudinaryServiceTests
{
    [Fact]
    public async Task UploadImageAsync_ShouldReturnSecureUrl()
    {
        // Arrange
        // 1. T?o mock cho Cloudinary
        var mockCloudinary = new Mock<Cloudinary>();

        // T?o m?t instance c? th? c?a ImageUploadParams
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription("test-image.jpg", new MemoryStream()),
            Folder = "test-folder"
        };

        mockCloudinary.Setup(c => c.UploadAsync(uploadParams))
            .ReturnsAsync(new ImageUploadResult { SecureUrl = new System.Uri("https://res.cloudinary.com/test/image/upload/v123/test-image.jpg") });

        // 2. T?o mock cho IFormFile
        var mockFile = new Mock<IFormFile>();
        var stream = new MemoryStream(new byte { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 });
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.FileName).Returns("test-image.jpg");
        mockFile.Setup(f => f.Length).Returns(stream.Length);

        // 3. Kh?i t?o CloudinaryService v?i mock Cloudinary
        var cloudinaryService = new CloudinaryService(mockCloudinary.Object);

        // Act
        var result = await cloudinaryService.UploadImageAsync(mockFile.Object, "test-folder");

        // Assert
        Assert.Equal("https://res.cloudinary.com/test/image/upload/v123/test-image.jpg", result);
    }
}