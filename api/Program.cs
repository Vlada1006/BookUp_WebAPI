using System.Diagnostics;
using System.Text.Json.Serialization;
using api.Data;
using api.Interfaces;
using api.Models.OtherModels;
using api.Repositories;
using api.Services;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(u => u.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

builder.Services.AddScoped<ILocationCategoryInterface, LocationCategoryRepository>();
builder.Services.AddScoped<ILocationInterface, LocationRepository>();
builder.Services.AddScoped<IPlaceInterface, PlaceRepository>();

builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<PlaceAvailabilityService>();

var app = builder.Build();

app.UseRouting();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookUp Web API");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();

