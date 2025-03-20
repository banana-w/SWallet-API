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


        public PaymentController(IVnpay vnpay, IOptions<VnpayConfig> vnpayConfig, IPointPackageService pointPackageService, ICampusService campusService, IWalletService walletService)
        {
            _vnpay = vnpay;
            _vnpayConfig = vnpayConfig.Value;
            _pointPackageService = pointPackageService;
            _campusService = campusService;
            _walletService = walletService;
        }

        [HttpPost("purchase-points")]
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

        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    if (paymentResult.IsSuccess)
                    {
                        var orderInfo = paymentResult.PaymentResponse.Description;
                        var parts = orderInfo.Split('-');
                        var campusId = parts[0];
                        var pointPackageId = parts[1];

                        // Lấy thông tin gói điểm và Campus (nếu có)
                        var pointPackage = await _pointPackageService.GetPointPackageById(pointPackageId);
                        var campus = await _campusService.GetCampusById(campusId);

                        // Cập nhật Wallet (nếu có thông tin)
                        if (campus != null && pointPackage != null)
                        {
                            await _walletService.AddPointsToWallet(campus.Id, (int)pointPackage.Point);
                        }

                        return Ok(); // Trả về 200 OK
                    }
                    else
                    {
                        return BadRequest("Thanh toán thất bại"); // Trả về 400 BadRequest
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message); // Trả về 400 BadRequest
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán."); // Trả về 404 NotFound
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
        public ActionResult<string> Callback()
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






