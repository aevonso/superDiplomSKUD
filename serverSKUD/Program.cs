using System;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
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

// 1. Add framework services
builder.Services.AddControllers();

// 2. Swagger / OpenAPI
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

// 3. Configure JWT settings from configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

// 4. Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

var connString = builder.Configuration.GetConnectionString("pgSql");
ArgumentNullException.ThrowIfNull(connString, nameof(connString));
builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(connString)
);

// 6. Application dependencies
// Authorization
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQueryService<EntryDto, AuthResult>, AutorizationQueryService>();
builder.Services.AddScoped<IQueryService<RefreshDto, AuthResult>, RefreshQueryService>();
builder.Services.AddScoped<IQueryService<Employeer, ClaimsPrincipal>, CreatePrincipalQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();


// QR‑code
builder.Services.AddScoped<IQrRepository, QrRepository>();
builder.Services.AddScoped<ICommandService<GenerateQrDto>, GenerateQrCommandService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "СКУД НАТК API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
