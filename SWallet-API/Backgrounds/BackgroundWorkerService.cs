using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Interfaces.CampaignRepository;
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

                var campaignRepository = scope.ServiceProvider.GetRequiredService<ICampaignRepository>();

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
                        _logger.LogInformation("Checking campaign {CampaignId}: Status={Status}, StartOn={StartOn}, EndOn={EndOn}, CurrentTime={Now}",
                            campaign.Id, campaign.Status, campaign.StartOn, campaign.EndOn, DateTime.UtcNow);

                        // Chuyển đổi StartOn và EndOn sang DateTime để dễ so sánh
                        DateTime? startDate = campaign.StartOn?.ToDateTime(TimeOnly.MinValue);
                        DateTime? endDate = campaign.EndOn?.ToDateTime(TimeOnly.MinValue);

                        // Nếu cả StartOn và EndOn đều đã qua, campaign đã hoàn thành
                        bool isCompleted = startDate.HasValue && endDate.HasValue &&
                                          startDate <= now && endDate <= now;

                        if (isCompleted)
                        {
                            // Đảm bảo campaign được deactive nếu đã hoàn thành
                            if (campaign.Status == (int)CampaignStatus.Active)
                            {
                                await campaignRepository.UpdateStatusAsync(campaign.Id, false);
                                hasChanges = true;
                                _logger.LogInformation("Campaign {CampaignId} has completed. Status updated to false.", campaign.Id);

                                // Gửi email thông báo kết thúc
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
                                        _logger.LogInformation("Email sent for campaign {CampaignId} completion.", campaign.Id);
                                    }
                                }
                            }
                            continue; // Bỏ qua các xử lý khác cho campaign đã hoàn thành
                        }

                        // Xử lý campaign chưa hoàn thành
                        if (campaign.Status == (int)CampaignStatus.Inactive && startDate.HasValue && startDate <= now)
                        {
                            await campaignRepository.UpdateStatusAsync(campaign.Id, true);
                            hasChanges = true;
                            _logger.LogInformation("Campaign {CampaignId} has started. Status updated to true.", campaign.Id);

                            if (campaign?.Brand?.Account?.Email != null)
                            {
                                bool emailSent = emailService.SendEmailCamapaign(
                                    true,
                                    campaign.Brand.Account.Email,
                                    campaign.Brand.BrandName,
                                    campaign.CampaignName,
                                    null);

                                if (emailSent)
                                {
                                    _logger.LogInformation("Email sent for campaign {CampaignId} start notification.", campaign.Id);
                                }
                            }
                        }
                        else if (campaign.Status == (int)CampaignStatus.Active && endDate.HasValue && endDate <= now)
                        {
                            await campaignRepository.UpdateStatusAsync(campaign.Id, false);
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
                            }
                        }
                    }

                    if (hasChanges)
                    {
                        try
                        {
                            await unitOfWork.CommitAsync();
                            _logger.LogInformation("Changes committed to database.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to commit changes to database.");
                            throw; // Ném lại để dễ debug
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("No campaigns found to process.");
                }
                // Để triển khai thực tế:
                await Task.Delay((int)delay.TotalMilliseconds, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("BackgroundWorkerService is stopping.");
    }
}
