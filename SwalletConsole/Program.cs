
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Implement;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Services.Implements;

internal class Program
{
    private static void Main(string[] args)
    {
        SwalletDbContext dbContext = new();
        UnitOfWork<SwalletDbContext> unitOfWork = new(dbContext);
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
        ILogger<EmailService> logger = new Logger<EmailService>(new LoggerFactory());
        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        EmailService emailService = new(unitOfWork, logger,  httpContextAccessor,configuration);
        var a = configuration["EmailConfig:Host"];
        emailService.SendEmailStudentRegister("huyln2904@gmail.com");
    }
}