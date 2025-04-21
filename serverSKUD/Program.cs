using System;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using AutorizationDomain;
using AutorizationDomain.Queries;
using AutorizationDomain.Queries.Object;
using Data;
using Data.Repository;
using serviceSKUD;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:49682")   
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "СКУД НАТК API",
        Version = "v1",
        Description = "API для системы контроля и управления доступом в НАТК"
    });
});

// 4. Настройки JWT из appsettings.json
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

// 5. Аутентификация по JWT
builder.Services
    .AddAuthentication("JwtBearer")
    .AddJwtBearer("JwtBearer", opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var connString = builder.Configuration.GetConnectionString("pgSql");
ArgumentNullException.ThrowIfNull(connString, nameof(connString));
builder.Services.AddDbContext<Connection>(opt =>
    opt.UseNpgsql(connString)
);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQueryService<EntryDto, AuthResult>, AutorizationQueryService>();
builder.Services.AddScoped<IQueryService<RefreshDto, AuthResult>, RefreshQueryService>();
builder.Services.AddScoped<IQueryService<Employeer, ClaimsPrincipal>, CreatePrincipalQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IQrRepository, QrRepository>();
builder.Services.AddScoped<ICommandService<GenerateQrDto>, GenerateQrCommandService>();

var app = builder.Build();

// 8. Swagger UI в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "СКУД НАТК API v1");
    });
}

app.UseHttpsRedirection();

// 9. Подключаем CORS _до_ аутентификации и контроллеров
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// 10. Маршрутизация контроллеров
app.MapControllers();

app.Run();
