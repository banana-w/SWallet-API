using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.QRCodeRequest;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Payload.Response.QRCodeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IQRCodeService
    {
        Task<QRCodeResponse> GenerateQRCode(GenerateQRCodeRequest request);

        Task<ScanQRCodeResponse> ScanQRCode(ScanQRCodeRequest request);

        Task<IPaginate<QrCodeHistoryResponse>> GetQrHistoryByLectureId(string lectureId, string searchName, int page, int size);
    }
}
