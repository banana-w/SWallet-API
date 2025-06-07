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
using SWallet.Domain.Paginate;
using Microsoft.EntityFrameworkCore;
using SWallet.Repository.Payload.Response.Lecturer;
using System.Linq.Expressions;



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

        public async Task<IPaginate<QRCodeUsageHistoryResponse>> GetQRCodeUsageHistory(
            string lecturerId,
            string searchName,
            int page,
            int size)
        {
            if (string.IsNullOrEmpty(lecturerId))
            {
                throw new ApiException("LecturerId cannot be empty", 400, "BAD_REQUEST");
            }

            // Filter QrcodeUsage records by lecturerId in QrcodeJson
            Expression<Func<QrcodeUsage, bool>> filterQuery = u =>
                u.QrcodeJson.Contains($"\"LecturerId\":\"{lecturerId}\"");

            var qrCodeUsageRepository = _unitOfWork.GetRepository<QrcodeUsage>();
            var usages = await qrCodeUsageRepository.GetPagingListAsync(
                selector: x => x,
                predicate: filterQuery,
                orderBy: q => q.OrderByDescending(u => u.UsedAt),
                page: page,
                size: size
            );

            // Convert to QRCodeUsageHistoryResponse
            var history = new List<QRCodeUsageHistoryResponse>();
            foreach (var usage in usages.Items)
            {
                GenerateQRCodeRequest qrCodeData;
                try
                {
                    qrCodeData = JsonConvert.DeserializeObject<GenerateQRCodeRequest>(usage.QrcodeJson);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Failed to deserialize QRCode JSON: {QRCodeJson}", usage.QrcodeJson);
                    continue;
                }

                var student = await _studentService.GetStudentAsync(usage.StudentId);
                if (student == null)
                {
                    _logger.LogWarning("Student not found: {StudentId}", usage.StudentId);
                    continue;
                }

                // Filter by student name if searchName is provided
                if (!string.IsNullOrEmpty(searchName) &&
                    !student.FullName.ToLower().Contains(searchName.ToLower()))
                {
                    continue;
                }

                history.Add(new QRCodeUsageHistoryResponse
                {
                    StudentId = usage.StudentId,
                    StudentName = student.FullName,
                    PointsTransferred = qrCodeData.Points,
                    UsedAt = (DateTime)usage.UsedAt
                });
            }

            // Sort the final history list by UsedAt in descending order
            history = history.OrderByDescending(h => h.UsedAt).ToList();

            return new Paginate<QRCodeUsageHistoryResponse>(
                history,
                page,
                size,
                1
            );
        }

        public async Task<QRCodeResponse> GenerateQRCode(GenerateQRCodeRequest request)
        {
            // Kiểm tra request có null không
            if (request == null)
            {
                throw new ApiException("Yêu cầu không được để trống", 400, "BAD_REQUEST");
            }

            // Kiểm tra MaxUsageCount hợp lệ
            if (request.MaxUsageCount <= 0)
            {
                throw new ApiException("Số lần sử dụng tối đa phải lớn hơn 0", 400, "INVALID_MAX_USAGE_COUNT");
            }

            // Kiểm tra _cloudinary có được khởi tạo không
            if (_cloudinary == null)
            {
                throw new ApiException("Dịch vụ Cloudinary chưa được khởi tạo", 500, "INTERNAL_SERVER_ERROR");
            }

            var lecturerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: w => w.LecturerId == request.LecturerId
            );

            if (lecturerWallet == null || lecturerWallet.Balance == null)
            {
                throw new ApiException("Không tìm thấy ví cho giảng viên này", 404, "NOT_FOUND");
            }

            if (lecturerWallet.Balance < request.Points)
            {
                throw new ApiException(
                    $"Số dư không đủ. Yêu cầu: {request.Points}, Số dư hiện tại: {lecturerWallet.Balance}",
                    400,
                    "INSUFFICIENT_BALANCE"
                );
            }
            var vnTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone());
            var availableTime = vnTime.AddHours(request.AvailableHours);
            var qrCodeJson = JsonConvert.SerializeObject(new
            {
                lecturerId = request.LecturerId,
                points = request.Points,
                startOnTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone()),
                expirationTime = availableTime,
                availableHours = request.AvailableHours,
                longitude = request.Longitude,
                latitude = request.Latitude,
                maxUsageCount = request.MaxUsageCount // Lưu số lần tối đa
            });

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeJson, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);
                if (qrCodeImageBytes == null || qrCodeImageBytes.Length == 0)
                {
                    throw new ApiException("Không thể tạo hình ảnh mã QR", 500, "INTERNAL_SERVER_ERROR");
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
                        throw new ApiException("Không thể tải mã QR lên Cloudinary", 500, "INTERNAL_SERVER_ERROR");
                    }

                    // Lưu lịch sử tạo mã QR
                    var qrCodeHistory = new QrCodeHistory
                    {
                        Id = Ulid.NewUlid().ToString(),
                        LectureId = request.LecturerId,
                        Points = request.Points,
                        StartOnTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone()),
                        ExpirationTime = availableTime,
                        QrCodeData = qrCodeJson,
                        QrCodeImageUrl = uploadResult.Url.ToString(),
                        Longtitude = (decimal?)request.Longitude,
                        Latitude = (decimal?)request.Latitude,
                        MaxUsageCount = request.MaxUsageCount, // Lưu số lần tối đa
                        CurrentUsageCount = 0, // Khởi tạo số lần đã sử dụng
                        CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone())
                    };

                    await _unitOfWork.GetRepository<QrCodeHistory>().InsertAsync(qrCodeHistory);
                    await _unitOfWork.CommitAsync();

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
                throw new ApiException("Dữ liệu yêu cầu không hợp lệ", 400, "BAD_REQUEST");
            }

           

            // Kiểm tra xem mã QR đã được sử dụng chưa
            var qrCodeUsageRepository = _unitOfWork.GetRepository<QrcodeUsage>();
            var isQrCodeUsed = await qrCodeUsageRepository.AnyAsync(
                predicate: u => u.QrcodeJson == request.QRCodeJson && u.StudentId == request.StudentId
            );
            if (isQrCodeUsed)
            {
                _logger.LogWarning("Mã QR đã được sử dụng: {QRCodeJson} bởi Sinh viên: {StudentId}", request.QRCodeJson, request.StudentId);
                throw new ApiException("Mã QR này đã được sử dụng", 400, "QRCODE_ALREADY_USED");
            }

            // Giải mã dữ liệu từ mã QR
            GenerateQRCodeRequest qrCodeData;
            try
            {
                qrCodeData = JsonConvert.DeserializeObject<GenerateQRCodeRequest>(request.QRCodeJson);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Không thể giải mã JSON của mã QR: {QRCodeJson}", request.QRCodeJson);
                throw new ApiException("Định dạng mã QR không hợp lệ", 400, "INVALID_QRCODE");
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(qrCodeData.LecturerId) || qrCodeData.Points <= 0 || qrCodeData.MaxUsageCount <= 0)
            {
                throw new ApiException("Dữ liệu mã QR không đầy đủ", 400, "INVALID_QRCODE");
            }

            // Kiểm tra thời gian hiệu lực
            if (DateTime.Now < qrCodeData.StartOnTime || DateTime.Now > qrCodeData.ExpirationTime)
            {
                throw new ApiException("Mã QR đã hết hạn hoặc chưa khả dụng", 400, "EXPIRED_QRCODE");
            }

            // Kiểm tra số lần sử dụng
            var qrCodeHistory = await _unitOfWork.GetRepository<QrCodeHistory>().SingleOrDefaultAsync(
                predicate: h => h.QrCodeData == request.QRCodeJson
            );
            if (qrCodeHistory == null)
            {
                throw new ApiException("Không tìm thấy lịch sử mã QR", 404, "QRCODE_NOT_FOUND");
            }

            if (qrCodeHistory.CurrentUsageCount >= qrCodeHistory.MaxUsageCount)
            {
                throw new ApiException(
                    $"Mã QR đã đạt số lần sử dụng tối đa: {qrCodeHistory.MaxUsageCount}",
                    400,
                    "MAX_USAGE_REACHED"
                );
            }

            // Kiểm tra khoảng cách
            const double maxDistanceMeters = 1000; // Ngưỡng khoảng cách tối đa (100 mét)
            var distance = CalculateDistance(qrCodeData.Latitude, qrCodeData.Longitude, request.Latitude, request.Longitude);
            if (distance > maxDistanceMeters)
            {
                throw new ApiException(
                    $"Sinh viên không ở gần vị trí mã QR. Khoảng cách: {distance:F2} mét, tối đa cho phép: {maxDistanceMeters} mét",
                    400,
                    "LOCATION_MISMATCH"
                );
            }

            // Lấy thông tin giảng viên và sinh viên
            var lecturer = await _lecturerService.GetLecturerById(qrCodeData.LecturerId);
            var student = await _studentService.GetStudentAsync(request.StudentId);

            if (lecturer == null || student == null)
            {
                throw new ApiException("Không tìm thấy giảng viên hoặc sinh viên", 404, "NOT_FOUND");
            }

            // Lấy ví của giảng viên và sinh viên
            var lecturerWallet = await _walletService.GetWalletByLecturerId(qrCodeData.LecturerId, 1);
            var studentWallet = await _walletService.GetWalletByStudentId(request.StudentId, 1);

            if (lecturerWallet == null || studentWallet == null)
            {
                throw new ApiException("Không tìm thấy ví của giảng viên hoặc sinh viên", 404, "NOT_FOUND");
            }

            if (lecturerWallet.Balance < qrCodeData.Points)
            {
                throw new ApiException(
                    $"Số dư ví giảng viên không đủ. Yêu cầu: {qrCodeData.Points}, Số dư hiện tại: {lecturerWallet.Balance}",
                    400,
                    "INSUFFICIENT_BALANCE"
                );
            }

            // Chuyển điểm
            await _walletService.UpdateWallet(lecturerWallet.Id, (int)(lecturerWallet.Balance - qrCodeData.Points));
            await _walletService.UpdateWallet(studentWallet.Id, (int)(studentWallet.Balance + qrCodeData.Points));

            // Lưu lịch sử sử dụng mã QR
            var qrCodeUsage = new QrcodeUsage
            {
                QrcodeJson = request.QRCodeJson,
                StudentId = request.StudentId,
                UsedAt = DateTime.Now,
                Longtitude = (decimal?)request.Longitude,
                Latitude = (decimal?)request.Latitude
            };
            await _unitOfWork.GetRepository<QrcodeUsage>().InsertAsync(qrCodeUsage);

            // Cập nhật số lần sử dụng
            qrCodeHistory.CurrentUsageCount += 1;
             _unitOfWork.GetRepository<QrCodeHistory>().UpdateAsync(qrCodeHistory);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Đã chuyển {Points} điểm từ Giảng viên {LecturerId} sang Sinh viên {StudentId}",
                qrCodeData.Points, qrCodeData.LecturerId, request.StudentId);

            return new ScanQRCodeResponse
            {
                StudentId = request.StudentId,
                PointsTransferred = qrCodeData.Points,
                NewBalance = (int)studentWallet.Balance + qrCodeData.Points
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Bán kính Trái Đất (mét)
            double lat1Rad = lat1 * Math.PI / 180;
            double lat2Rad = lat2 * Math.PI / 180;
            double deltaLat = (lat2 - lat1) * Math.PI / 180;
            double deltaLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Khoảng cách tính bằng mét
        }

        public async Task<IPaginate<QrCodeHistoryResponse>> GetQrHistoryByLectureId(string lectureId, string searchName, int page, int size)
        {
            if (string.IsNullOrEmpty(lectureId))
            {
                throw new ApiException("LectureId cannot be empty", 400, "BAD_REQUEST");
            }
            Expression<Func<QrCodeHistory, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.LectureId == lectureId;
            }
            else
            {
                filterQuery = p => p.LectureId == lectureId && p.LectureId.Contains(searchName);
            }

            var history = await _unitOfWork.GetRepository<QrCodeHistory>().GetPagingListAsync(
                selector: x => new QrCodeHistoryResponse
                {
                    Id = x.Id,
                    LecturerId = x.LectureId,
                    Points = x.Points,
                    StartOnTime = x.StartOnTime,
                    ExpirationTime = x.ExpirationTime,
                    QRCodeData = x.QrCodeData,
                    QRCodeImageUrl = x.QrCodeImageUrl,
                    CreatedAt = x.CreatedAt,
                    MaxUsageCount = x.MaxUsageCount,
                    CurrentUsageCount = x.CurrentUsageCount
                },
                predicate: filterQuery,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                page: page,
                size: size);

            return history;
        }

        public class QRCodeUsageHistoryResponse
        {
            public string StudentId { get; set; }
            public string StudentName { get; set; }
            public int PointsTransferred { get; set; }
            public DateTime UsedAt { get; set; }
        }
    }
}
