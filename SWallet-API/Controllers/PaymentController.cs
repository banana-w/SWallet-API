using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using Microsoft.Extensions.Options;
using SWallet.Repository.VNPAY;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.PointPackage;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Services.Implements;
using SWallet.Domain.Models;


namespace SWallet_API.Controllers
{
    [Route("api/VNPAY")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IVnpay _vnpay;
        private readonly VnpayConfig _vnpayConfig;
        private readonly IPointPackageService _pointPackageService;
        private readonly ICampusService _campusService;
        private readonly IWalletService _walletService;
        private readonly IBrandService _brandService;
        private readonly IPurchaseHistory _pointPurchaseHistoryService;


        public PaymentController(IVnpay vnpay, IOptions<VnpayConfig> vnpayConfig, IPointPackageService pointPackageService, ICampusService campusService, IWalletService walletService, IBrandService brandService, IPurchaseHistory pointPurchaseHistoryService)
        {
            _vnpay = vnpay;
            _vnpayConfig = vnpayConfig.Value;
            _pointPackageService = pointPackageService;
            _campusService = campusService;
            _walletService = walletService;
            _brandService = brandService;
            _pointPurchaseHistoryService = pointPurchaseHistoryService;
        }


        [HttpGet("get-purchase-history")]
        public async Task<IActionResult> GetPurchaseHistoryById(string id)
        {
            var historyResponse = await _pointPurchaseHistoryService.GetPurchaseHistoryById(id);
            if (historyResponse == null)
            {
                return NotFound();
            }
            return Ok(historyResponse);
        }

        [HttpPost("campus-purchase-points")]
        public async Task<IActionResult> PurchasePoints([FromBody] PurchasePointRequest request)
        {
            try
            {
                // Lấy thông tin gói điểm
                var pointPackage = await _pointPackageService.GetPointPackageById(request.PointPackageId);
                if (pointPackage == null)
                {
                    return BadRequest(new { error = "Point package not found" });
                }

                var campus = await _campusService.GetCampusById(request.CampusId);
                if (campus == null)
                {
                    return BadRequest(new { error = "Campus not found" });
                }
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                var orderInfo = $"{request.CampusId}-{request.PointPackageId}";

                var paymentRequest = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Description = orderInfo,
                    Money = (double)pointPackage.Price,
                    IpAddress = ipAddress, // Lấy địa chỉ IP (sử dụng helper method hoặc inject IHttpContextAccessor)
                    BankCode = BankCode.ANY, // Hoặc giá trị cụ thể
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND, // Hoặc giá trị cụ thể
                    Language = DisplayLanguage.Vietnamese // Hoặc giá trị cụ thể
                };

                // Lưu lịch sử giao dịch
                var purchaseHistory = new PointPurchaseHistory
                {
                    Id = Ulid.NewUlid().ToString(),
                    EntityId = request.CampusId,
                    EntityType = "Campus",
                    PointPackageId = request.PointPackageId,
                    Points = (int)pointPackage.Point,
                    Amount = (decimal)pointPackage.Price,
                    PaymentId = paymentRequest.PaymentId,
                    PaymentStatus = "Pending",
                    CreatedDate = DateTime.Now
                };
                await _pointPurchaseHistoryService.SavePurchaseHistoryAsync(purchaseHistory);

                // Tạo link thanh toán
                var paymentUrl = _vnpay.GetPaymentUrl(paymentRequest);

                // Trả về link thanh toán
                return Ok(new { paymentUrl });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("brand-purchase-points")]
        public async Task<IActionResult> BrandPurchasePoints([FromBody] PurchasePointRequest request)
        {
            try
            {
                // Lấy thông tin gói điểm
                var pointPackage = await _pointPackageService.GetPointPackageById(request.PointPackageId);
                if (pointPackage == null)
                {
                    return BadRequest(new { error = "Point package not found" });
                }

                var brand = await _brandService.GetBrandById(request.BrandId, null);
                if (brand == null)
                {
                    return BadRequest(new { error = "Brand not found" });
                }
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                var orderInfo = $"{request.BrandId}-{request.PointPackageId}";

                var paymentRequest = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Description = orderInfo,
                    Money = (double)pointPackage.Price,
                    IpAddress = ipAddress, // Lấy địa chỉ IP (sử dụng helper method hoặc inject IHttpContextAccessor)
                    BankCode = BankCode.ANY, // Hoặc giá trị cụ thể
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND, // Hoặc giá trị cụ thể
                    Language = DisplayLanguage.Vietnamese // Hoặc giá trị cụ thể
                };

                // Lưu lịch sử giao dịch
                var purchaseHistory = new PointPurchaseHistory
                {
                    Id = Ulid.NewUlid().ToString(),
                    EntityId = request.BrandId,
                    EntityType = "Brand",
                    PointPackageId = request.PointPackageId,
                    Points = (int)pointPackage.Point,
                    Amount = (decimal)pointPackage.Price,
                    PaymentId = paymentRequest.PaymentId,
                    PaymentStatus = "Pending",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                    
                };
                await _pointPurchaseHistoryService.SavePurchaseHistoryAsync(purchaseHistory);

                // Tạo link thanh toán
                var paymentUrl = _vnpay.GetPaymentUrl(paymentRequest);

                // Trả về link thanh toán
                return Ok(new { paymentUrl });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    // Tìm giao dịch trong lịch sử
                    var purchaseHistory = await _pointPurchaseHistoryService.GetPurchaseHistoryByPaymentIdAsync(paymentResult.PaymentId.ToString());
                    if (purchaseHistory == null)
                    {
                        return BadRequest(new { error = "Purchase history not found" });
                    }

                    // Cập nhật trạng thái giao dịch
                    purchaseHistory.PaymentStatus = paymentResult.IsSuccess ? "Success" : "Failed";
                    purchaseHistory.UpdatedDate = DateTime.Now;
                    await _pointPurchaseHistoryService.UpdatePurchaseHistoryAsync(purchaseHistory);

                    if (paymentResult.IsSuccess)
                    {
                        var orderInfo = paymentResult.Description;
                        var parts = orderInfo.Split('-');
                        if (parts.Length != 2)
                        {
                            return BadRequest(new { error = "Invalid order info format" });
                        }

                        var entityId = parts[0];
                        var pointPackageId = parts[1];

                        // Lấy thông tin gói điểm
                        var pointPackage = await _pointPackageService.GetPointPackageById(pointPackageId);
                        if (pointPackage == null)
                        {
                            return BadRequest(new { error = "Point package not found" });
                        }

                        // Kiểm tra xem entityId là Campus hay Brand
                        var campus = await _campusService.GetCampusById(entityId);
                        if (campus != null)
                        {
                            // Trường hợp Campus mua điểm
                            await _walletService.AddPointsToWallet(campus.Id, (int)pointPackage.Point);
                            return Ok();
                        }

                        var brand = await _brandService.GetBrandById(entityId, null);
                        if (brand != null)
                        {
                            // Trường hợp Brand mua điểm
                            await _walletService.AddPointsToBrandWallet(brand.Id, (int)pointPackage.Point);
                            return Ok();
                        }

                        return BadRequest(new { error = "Campus or Brand not found" });
                    }
                    else
                    {
                        return BadRequest(new { error = "Thanh toán thất bại" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }

            return NotFound(new { error = "Không tìm thấy thông tin thanh toán" });
        }


        //[HttpGet("IpnAction")]
        //public async Task<IActionResult> IpnAction()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _vnpay.GetPaymentResult(Request.Query);

        //            if (paymentResult.IsSuccess)
        //            {
        //                var orderInfo = paymentResult.Description;
        //                var parts = orderInfo.Split('-');
        //                if (parts.Length != 2)
        //                {
        //                    return BadRequest(new { error = "Invalid order info format" });
        //                }

        //                var entityId = parts[0]; // Có thể là CampusId hoặc BrandId
        //                var pointPackageId = parts[1];

        //                // Lấy thông tin gói điểm
        //                var pointPackage = await _pointPackageService.GetPointPackageById(pointPackageId);
        //                if (pointPackage == null)
        //                {
        //                    return BadRequest(new { error = "Point package not found" });
        //                }

        //                // Kiểm tra xem entityId là Campus hay Brand
        //                var campus = await _campusService.GetCampusById(entityId);
        //                if (campus != null)
        //                {
        //                    // Trường hợp Campus mua điểm
        //                    await _walletService.AddPointsToWallet(campus.Id, (int)pointPackage.Point);
        //                    return Ok(); // Trả về 200 OK
        //                }

        //                var brand = await _brandService.GetBrandById(entityId, null);
        //                if (brand != null)
        //                {
        //                    // Trường hợp Brand mua điểm
        //                    await _walletService.AddPointsToBrandWallet(brand.Id, (int)pointPackage.Point);
        //                    return Ok(); // Trả về 200 OK
        //                }

        //                return BadRequest(new { error = "Campus or Brand not found" });
        //            }
        //            else
        //            {
        //                return BadRequest(new { error = "Thanh toán thất bại" });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { error = ex.Message });
        //        }
        //    }

        //    return NotFound(new { error = "Không tìm thấy thông tin thanh toán" });
        //}


        //[HttpGet("IpnAction")]
        //public async Task<IActionResult> IpnAction()
        //{
            
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _vnpay.GetPaymentResult(Request.Query);

        //            if (paymentResult.IsSuccess)
        //            {
        //                var orderInfo = paymentResult.Description;
        //                var parts = orderInfo.Split('-');
        //                var campusId = parts[0];
        //                var pointPackageId = parts[1];

        //                // Lấy thông tin gói điểm và Campus (nếu có)
        //                var pointPackage = await _pointPackageService.GetPointPackageById(pointPackageId);
        //                var campus = await _campusService.GetCampusById(campusId);

        //                // Cập nhật Wallet (nếu có thông tin)
        //                if (campus != null && pointPackage != null)
        //                {
        //                    await _walletService.AddPointsToWallet(campus.Id, (int)pointPackage.Point);
        //                }

        //                return Ok(); // Trả về 200 OK
        //            }
        //            else
        //            {
        //                return BadRequest(error: "Thanh toán thất bại"); // Trả về 400 BadRequest
                       
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message); // Trả về 400 BadRequest

        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán."); // Trả về 404 NotFound
        //}




        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //    [HttpGet("Callback")]
        //    public async Task<ActionResult<string>> Callback()
        //    {
        //        if (Request.QueryString.HasValue)
        //        {
        //            try
        //            {
        //                var paymentResult = _vnpay.GetPaymentResult(Request.Query);

        //                if (paymentResult.IsSuccess)
        //                {

        //                    var orderInfo = paymentResult.Description;
        //                    var parts = orderInfo.Split('-');
        //                    var campusId = parts[0];
        //                    var pointPackageId = parts[1];

        //                    // Lấy thông tin gói điểm và Campus (nếu có)
        //                    var pointPackage = await _pointPackageService.GetPointPackageById(pointPackageId);
        //                    var campus = await _campusService.GetCampusById(campusId);
        //                    var resultDescription = $"Thanh toán thành công. Đã cộng thêm {pointPackage.Point} vào ví của Campus với ID là: {campus.Id}.";
        //                    // Cập nhật Wallet (nếu có thông tin)
        //                    if (campus != null && pointPackage != null)
        //                    {
        //                        await _walletService.AddPointsToWallet(campus.Id, (int)pointPackage.Point);
        //                    }

        //                    return Ok(resultDescription); // Trả về 200 OK
        //                }
        //                else
        //                {
        //                    return BadRequest("Thanh toán thất bại"); // Trả về 400 BadRequest
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest(ex.Message); // Trả về 400 BadRequest
        //            }
        //        }

        //        return NotFound("Không tìm thấy thông tin thanh toán."); // Trả về 404 NotFound
        //    }
        //}


        [HttpGet("Callback")]
        public async Task<ActionResult<string>> Callback()
        {

            if (Request.QueryString.HasValue)
            {

                try
                {

                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                    if (paymentResult.IsSuccess)
                    {
                        return Ok(resultDescription);
                    }

                    return BadRequest(resultDescription);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}






