using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;

namespace SWallet_API.Backgrounds;

public class BackgroundWorkerService : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundWorkerService(
        ILogger<BackgroundWorkerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundWorkerService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Tính thời gian chờ đến 0:00 UTC ngày tiếp theo
                DateTime now = DateTime.UtcNow;
                DateTime nextRun = now.Date.AddDays(1); // 0:00 UTC ngày tiếp theo
                TimeSpan delay = nextRun - now;

                _logger.LogInformation("Worker scheduled to run next at: {time}. Waiting for {delay} ms", nextRun, delay.TotalMilliseconds);

                // Chờ đến thời điểm chạy tiếp theo


                await Task.Delay((int)delay.TotalMilliseconds, stoppingToken);

                // await Task.Delay(10000, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker stopped before processing.");
                    break;
                }

                _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);

                // Tạo scope để sử dụng các dịch vụ Scoped
                using var scope = _serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SwalletDbContext>>();

                // Lấy repository từ UnitOfWork trong scope
                var campaignRepository = unitOfWork.GetRepository<Campaign>();
                var campaignActivityRepository = unitOfWork.GetRepository<CampaignActivity>();

                // Lấy danh sách campaign với Status = false
                Expression<Func<Campaign, bool>> predicate = c => c.Status == false;
                List<Campaign> campaigns = (await campaignRepository.GetListAsync(
                    predicate: predicate,
                    include: q => q.Include(c => c.Brand).ThenInclude(b => b.Account)
                )).ToList();

                if (campaigns?.Count > 0)
                {
                    _logger.LogInformation("Found {count} campaigns to process.", campaigns.Count);

                    foreach (Campaign campaign in campaigns)
                    {
                        try
                        {
                            if (campaign?.Brand?.Account?.Email != null)
                            {
                                bool emailSent = emailService.SendEmailCamapaign(
                                    false, // Giả sử false là "Closed"
                                    campaign.Brand.Account.Email,
                                    campaign.Brand.BrandName,
                                    campaign.CampaignName,
                                    null);

                                if (emailSent)
                                {
                                    _logger.LogInformation("Email sent for campaign {CampaignId}", campaign.Id);
                                    // Ví dụ: Cập nhật trạng thái nếu cần
                                    // campaign.Status = true;
                                    // campaignRepository.Update(campaign);
                                }
                                else
                                {
                                    _logger.LogError("Failed to send email for campaign {CampaignId}", campaign.Id);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Campaign {CampaignId} has missing Brand or Account data.", campaign.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing campaign {CampaignId}", campaign.Id);
                        }
                    }

                    // Commit thay đổi nếu có
                    await unitOfWork.CommitAsync();
                    _logger.LogInformation("Changes committed to database.");
                }
                else
                {
                    _logger.LogInformation("No campaigns found to process.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BackgroundWorkerService.");
            }
        }

        _logger.LogInformation("BackgroundWorkerService is stopping.");
    }
}
