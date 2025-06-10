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
using AuntificationDomain.Queries.Object;
using AuntificationDomain;
using AuthenticationDomain.Queries;
using AuntificationDomain.Queries;
using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using DashboardDomain.Queries;
using EmployeeDomain.Queiries;
using System.Text.Json.Serialization;
using serverSKUD.Hubs;
using Xceed.Document.NET;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

//CORS
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
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP на всех интерфейсах порт 5201
    options.ListenAnyIP(5201);

    // HTTPS на всех интерфейсах порт 7267
    options.ListenAnyIP(7267, listenOpts =>
    {
        listenOpts.UseHttps();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    });

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

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

// Аутентификация по JWT
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
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings")
);
builder.Services.AddAuthorization();

var connString = builder.Configuration.GetConnectionString("pgSql");
ArgumentNullException.ThrowIfNull(connString, nameof(connString));
builder.Services.AddDbContext<Connection>(opt =>
    opt.UseNpgsql(connString)
);

//генерация отчетов
builder.Services.AddScoped<IGenerateReportService, GenerateReportQueryService>();

// Репозиторий из EmployeeDomain.IRepository
builder.Services.AddScoped<EmployeeDomain.IRepository.IEmployeeRepository, Data.Repository.EmployeePageRepository>();

// QueryService’ы из EmployeeDomain.Queiries

builder.Services.AddScoped<
    serviceSKUD.IQueryService<EmployeeDomain.Queiries.GetEmployeeByIdQuery, EmployeeDomain.Queiries.Object.EmployeeDto>,
    EmployeeDomain.Queiries.GetEmployeeByIdQueryService>();

builder.Services.AddScoped<
    serviceSKUD.IQueryService<EmployeeDomain.Queiries.GetEmployeesQuery, IEnumerable<EmployeeDomain.Queiries.Object.EmployeeDto>>,
    EmployeeDomain.Queiries.GetEmployeesQueryService>();

builder.Services.AddScoped<
    serviceSKUD.IQueryService<EmployeeDomain.Queiries.GetEmployeesCountQuery, int>,
    EmployeeDomain.Queiries.GetEmployeesCountQueryService>();



builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQueryService<EntryDto, AuthResult>, AutorizationQueryService>();
builder.Services.AddScoped<IQueryService<RefreshDto, AuthResult>, RefreshQueryService>();
builder.Services.AddScoped<IQueryService<Employeer, ClaimsPrincipal>, CreatePrincipalQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IQrRepository, QrRepository>();
builder.Services.AddScoped<ICommandService<GenerateQrDto>, GenerateQrCommandService>();


builder.Services.AddScoped<I2FaRepository, TwoFaRepository>();
builder.Services.AddScoped<
    IQueryService<TwoFactorGenerateDto, Task<string>>,
    Generate2FaQueryService>();
builder.Services.AddScoped<
    IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>>,
    Validate2FaQueryService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IMobileDeviceRepository, MobileDeviceRepository>();

builder.Services.AddScoped<IAccessAttemptRepository, AccessAttemptRepository>();
builder.Services.AddScoped<
    IQueryService<GetRecentAttemptsQuery, IEnumerable<AttemptDto>>,
    GetRecentAttemptsQueryService>();

builder.Services.AddScoped<
    IQueryService<GetDashboardStatsQuery, DashboardStatsDto>,
    GetDashboardStatsQueryService>();

builder.Services.AddScoped<
    IQueryService<GetFilteredAttemptsQuery, IEnumerable<AttemptDto>>,
    GetFilteredAttemptsQueryService>();
builder.Services.AddSignalR();

var app = builder.Build();

// В Development редиректа нет, в остальных средах — есть
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger UI только в Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "СКУД НАТК API v1");
    });
}

// ← УБРАЛИ второй app.UseHttpsRedirection()

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<LogHub>("/hubs/logs");
app.MapHub<MobileDeviceHub>("/hubs/devicestatus");
app.MapControllers();

app.Run();