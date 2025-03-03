using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SWallet.Repository.Payload;
using SWallet_API.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services
    .AddServices(builder.Configuration)
    .AddJwtValidation(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyDefaultPolicy",
        policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
});
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var cloudinarySettings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        cloudinarySettings.CloudName,
        cloudinarySettings.ApiKey,
        cloudinarySettings.ApiSecret));
});
builder.Services.AddHttpContextAccessor();

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
