using System.Diagnostics;
using System.Text.Json.Serialization;
using api.Data;
using api.Interfaces;
using api.Models;
using api.Models.OtherModels;
using api.Repositories;
using api.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOpenApi();
builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\nExample: Bearer abc123"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(u => u.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});


builder.Services.AddScoped<ILocationCategoryInterface, LocationCategoryRepository>();
builder.Services.AddScoped<ILocationInterface, LocationRepository>();
builder.Services.AddScoped<IPlaceInterface, PlaceRepository>();
builder.Services.AddScoped<IBookingInterface, BookingRepository>();

builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<PlaceAvailabilityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

builder.Services.AddHostedService<OldUserAccountCleanUpService>();

var app = builder.Build();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

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

