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
                // Tính thời gian chờ đến 00:01 UTC ngày tiếp theo
                DateTime now = DateTime.UtcNow;
                DateTime nextRun = now.Date.AddDays(1).AddMinutes(1); // 00:01 UTC ngày tiếp theo
                TimeSpan delay = nextRun - now;

                _logger.LogInformation("Worker scheduled to run next at: {time}. Waiting for {delay} ms", nextRun, delay.TotalMilliseconds);

                // Để triển khai thực tế:
                await Task.Delay((int)delay.TotalMilliseconds, stoppingToken);
                // Để test nhanh:
                //await Task.Delay(10000, stoppingToken);

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

                var campaignRepository = unitOfWork.GetRepository<Campaign>();
                var campaignActivityRepository = unitOfWork.GetRepository<CampaignActivity>();

                // Lấy tất cả campaign để kiểm tra cả StartOn và EndOn
                List<Campaign> allCampaigns = (await campaignRepository.GetListAsync(
                    include: q => q.Include(c => c.Brand).ThenInclude(b => b.Account)
                )).ToList();

                if (allCampaigns?.Count > 0)
                {
                    _logger.LogInformation("Found {count} campaigns to check.", allCampaigns.Count);
                    bool hasChanges = false;

                    foreach (Campaign campaign in allCampaigns)
                    {
                        // Kiểm tra StartOn: Nếu chưa active và đã đến ngày bắt đầu
                        if (campaign.Status == false && campaign.StartOn.HasValue &&
                            campaign.StartOn.Value.ToDateTime(TimeOnly.MinValue) <= DateTime.UtcNow)
                        {
                            campaign.Status = true;
                            hasChanges = true; // Đánh dấu có thay đổi để commit

                            _logger.LogInformation("Campaign {CampaignId} has started. Status updated to true.", campaign.Id);

                            if (campaign?.Brand?.Account?.Email != null)
                            {
                                bool emailSent = emailService.SendEmailCamapaign(
                                    true, // Trạng thái "Started"
                                    campaign.Brand.Account.Email,
                                    campaign.Brand.BrandName,
                                    campaign.CampaignName,
                                    null);

                                if (emailSent)
                                {
                                    _logger.LogInformation("Email sent for campaign {CampaignId} start notification.", campaign.Id);
                                }
                                else
                                {
                                    _logger.LogError("Failed to send email for campaign {CampaignId}", campaign.Id);
                                }
                            }
                        }
                        // Kiểm tra EndOn: Nếu đang active và đã đến ngày kết thúc
                        else if (campaign.Status == true && campaign.EndOn.HasValue &&
                                 campaign.EndOn.Value.ToDateTime(TimeOnly.MinValue) <= DateTime.UtcNow)
                        {
                            campaign.Status = false;
                            hasChanges = true; // Đánh dấu có thay đổi để commit

                            _logger.LogInformation("Campaign {CampaignId} has ended. Status updated to false.", campaign.Id);

                            if (campaign?.Brand?.Account?.Email != null)
                            {
                                bool emailSent = emailService.SendEmailCamapaign(
                                    false, // Trạng thái "Ended"
                                    campaign.Brand.Account.Email,
                                    campaign.Brand.BrandName,
                                    campaign.CampaignName,
                                    null);

                                if (emailSent)
                                {
                                    _logger.LogInformation("Email sent for campaign {CampaignId}", campaign.Id);
                                }
                                else
                                {
                                    _logger.LogError("Failed to send email for campaign {CampaignId}", campaign.Id);
                                }
                            }
                        }
                    }

                    if (hasChanges)
                    {
                        await unitOfWork.CommitAsync();
                        _logger.LogInformation("Changes committed to database.");
                    }
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