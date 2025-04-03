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

                // Chờ đến 00:01 UTC
                //await Task.Delay((int)delay.TotalMilliseconds, stoppingToken);
                // Để test nhanh
                await Task.Delay(10000, stoppingToken);

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

                // Lấy danh sách campaign đang hoạt động
                Expression<Func<Campaign, bool>> predicate = c => c.Status == true;
                List<Campaign> activeCampaigns = (await campaignRepository.GetListAsync(
                    predicate: predicate,
                    include: q => q.Include(c => c.Brand).ThenInclude(b => b.Account)
                )).ToList();

                if (activeCampaigns?.Count > 0)
                {
                    _logger.LogInformation("Found {count} active campaigns to check.", activeCampaigns.Count);
                    bool hasChanges = false;

                    foreach (Campaign campaign in activeCampaigns)
                    {
                        if (campaign.EndOn.HasValue && campaign.EndOn.Value.ToDateTime(TimeOnly.MinValue) <= DateTime.UtcNow)
                        {
                            campaign.Status = false;
                            campaignRepository.UpdateAsync(campaign);
                            hasChanges = true;

                            _logger.LogInformation("Campaign {CampaignId} has ended. Status updated to false.", campaign.Id);

                            if (campaign?.Brand?.Account?.Email != null)
                            {
                                bool emailSent = emailService.SendEmailCamapaign(
                                    false,
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
                    _logger.LogInformation("No active campaigns found to process.");
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