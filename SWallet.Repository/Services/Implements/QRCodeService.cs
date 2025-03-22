using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.QRCodeRequest;
using SWallet.Repository.Payload.Response.QRCodeResponse;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using SWallet.Repository.Payload.ExceptionModels;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;



namespace SWallet.Repository.Services.Implements
{
    public class QRCodeService : BaseService<QRCodeService>, IQRCodeService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILecturerService _lecturerService;
        private readonly IStudentService _studentService;
        private readonly IWalletService _walletService;

        public QRCodeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<QRCodeService> logger, Cloudinary cloudinary, ILecturerService lecturerService, IStudentService studentService, IWalletService walletService) : base(unitOfWork, logger)
        {
            _cloudinary = cloudinary;
            _lecturerService = lecturerService;
            _studentService = studentService;
            _walletService = walletService;
        }

        public async Task<QRCodeResponse> GenerateQRCode(GenerateQRCodeRequest request)
        {
            // Kiểm tra request có null không
            if (request == null)
            {
                throw new ApiException("Request cannot be null", 400, "BAD_REQUEST");
            }

            // Kiểm tra _cloudinary có được khởi tạo không
            if (_cloudinary == null)
            {
                throw new ApiException("Cloudinary service is not initialized", 500, "INTERNAL_SERVER_ERROR");
            }


            var lecturerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: w => w.LecturerId == request.LecturerId // Điều kiện tìm wallet theo LecturerId
            );

            if (lecturerWallet.Balance == null)
            {
                throw new ApiException("Wallet not found for this lecturer", 404, "NOT_FOUND");
            }

            if (lecturerWallet.Balance < request.Points)
            {
                throw new ApiException(
                    $"Insufficient balance. Required: {request.Points}, Available: {lecturerWallet.Balance}",
                    400,
                    "INSUFFICIENT_BALANCE"
                );
            }

            var availableTime = DateTime.Now.AddHours(request.AvailableHours);
            var qrCodeJson = JsonConvert.SerializeObject(new
            {
                lecturerId = request.LecturerId,
                points = request.Points,
                startOnTime = request.StartOnTime,
                availableHours = request.AvailableHours,
                expirationTime = availableTime
            });


            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeJson, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);
                if (qrCodeImageBytes == null || qrCodeImageBytes.Length == 0)
                {
                    throw new ApiException("Failed to generate QR code image", 500, "INTERNAL_SERVER_ERROR");
                }

                using (MemoryStream ms = new MemoryStream(qrCodeImageBytes))
                {
                    ms.Position = 0;

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription("qr_code.png", ms)
                    };

                    var uploadResult = _cloudinary.Upload(uploadParams);
                    if (uploadResult == null || uploadResult.Url == null)
                    {
                        throw new ApiException("Failed to upload QR code to Cloudinary", 500, "INTERNAL_SERVER_ERROR");
                    }

                    return new QRCodeResponse
                    {
                        QRCodeData = qrCodeJson,
                        QRCodeImageUrl = uploadResult.Url.ToString()
                    };
                }
            }
        }

        public async Task<ScanQRCodeResponse> ScanQRCode(ScanQRCodeRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.QRCodeJson) || string.IsNullOrEmpty(request.StudentId))
            {
                throw new ApiException("Invalid request data", 400, "BAD_REQUEST");
            }


            // Kiểm tra xem QRCode đã được sử dụng chưa
            var qrCodeUsageRepository = _unitOfWork.GetRepository<QrcodeUsage>();
            var isQrCodeUsed = await qrCodeUsageRepository.AnyAsync(
                predicate: u => u.QrcodeJson == request.QRCodeJson
            );
            if (isQrCodeUsed)
            {
                _logger.LogWarning("QRCode already used: {QRCodeJson} by Student: {StudentId}", request.QRCodeJson, request.StudentId);
                throw new ApiException("This QRCode has already been used", 400, "QRCODE_ALREADY_USED");
            }


            // Giải mã dữ liệu từ QRCode
            GenerateQRCodeRequest qrCodeData;
            try
            {
                qrCodeData = JsonConvert.DeserializeObject<GenerateQRCodeRequest>(request.QRCodeJson);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize QRCode JSON: {QRCodeJson}", request.QRCodeJson);
                throw new ApiException("Invalid QRCode format", 400, "INVALID_QRCODE");
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(qrCodeData.LecturerId) || qrCodeData.Points <= 0 || qrCodeData.ExpirationTime == null || qrCodeData.AvailableHours == null)
            {
                throw new ApiException("QRCode data is incomplete", 400, "INVALID_QRCODE");
            }
            // Kiểm tra thời gian hiệu lực
            if (DateTime.Now < qrCodeData.StartOnTime || DateTime.Now > qrCodeData.ExpirationTime)
            {
                throw new ApiException("QRCode has expired or is not yet available", 400, "EXPIRED_QRCODE");
            }

            // Lấy thông tin Lecturer và Student
            var lecturer = await _lecturerService.GetLecturerById(qrCodeData.LecturerId);
            var student = await _studentService.GetStudentAsync(request.StudentId);

            if (lecturer == null || student == null)
            {
                throw new ApiException("Lecturer or Student not found", 404, "NOT_FOUND");
            }

            // Lấy wallet của Lecturer và Student
            var lecturerWallet = await _walletService.GetWalletByLecturerId(qrCodeData.LecturerId, 1);
            var studentWallet = await _walletService.GetWalletByStudentId(request.StudentId, 1);

            if (lecturerWallet == null || studentWallet == null)
            {
                throw new ApiException("Lecturer or Student wallet not found", 404, "NOT_FOUND");
            }

            if (lecturerWallet.Balance < qrCodeData.Points)
            {
                throw new ApiException(
                    $"Insufficient balance in lecturer wallet. Required: {qrCodeData.Points}, Available: {lecturerWallet.Balance}",
                    400,
                    "INSUFFICIENT_BALANCE"
                );
            }

           

            // Chuyển điểm
            await _walletService.UpdateWallet(lecturerWallet.Id, (decimal)(lecturerWallet.Balance - qrCodeData.Points));
            await _walletService.UpdateWallet(studentWallet.Id, (decimal)(studentWallet.Balance + qrCodeData.Points));
           


            var qrCodeUsage = new QrcodeUsage
            {
                QrcodeJson = request.QRCodeJson,
                StudentId = request.StudentId,
                UsedAt = DateTime.Now
            };
            await _unitOfWork.GetRepository<QrcodeUsage>().InsertAsync(qrCodeUsage);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Transferred {Points} points from Lecturer {LecturerId} to Student {StudentId}",
            qrCodeData.Points, qrCodeData.LecturerId, request.StudentId);

            return new ScanQRCodeResponse
            {
                StudentId = request.StudentId,
                PointsTransferred = qrCodeData.Points,
                NewBalance = (decimal)studentWallet.Balance + qrCodeData.Points
            };
        }
    }
}
