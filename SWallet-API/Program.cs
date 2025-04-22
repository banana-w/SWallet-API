using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SWallet.Repository.Payload;
using SWallet.Repository.VNPAY;
using SWallet_API.Backgrounds;
using SWallet_API.Extentions;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services
    .AddServices(builder.Configuration)
    .AddJwtValidation(builder.Configuration);

builder.Services.AddRedisServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyDefaultPolicy",
        policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
});
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var cloudinarySettings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        cloudinarySettings.CloudName,
        cloudinarySettings.ApiKey,
        cloudinarySettings.ApiSecret));
});

builder.Services.Configure<VnpayConfig>(builder.Configuration.GetSection("Vnpay"));
builder.Services.AddSingleton<IVnpay>(sp =>
{
    var config = sp.GetRequiredService<IOptions<VnpayConfig>>().Value;
    var vnpay = new Vnpay();
    vnpay.Initialize(config.TmnCode, config.HashSecret, config.BaseUrl, config.ReturnUrl);
    return vnpay;
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Ghi log ra console
    logging.AddDebug();   // Ghi log ra debug output (nếu dùng IDE như Visual Studio)
});

builder.Services.AddHostedService<BackgroundWorkerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyDefaultPolicy");

app.UseCustomExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
