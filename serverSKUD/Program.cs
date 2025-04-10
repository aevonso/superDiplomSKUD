using AutorizationDomain;
using AutorizationDomain.Queries;
using AutorizationDomain.Queries.Object;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using serviceSKUD;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "СКУД НАТК API",
        Version = "v1",
        Description = "API для системы контроля и управления доступом в НАТК"
    });
});

// Получение строки подключения из конфигурации
string? connectionString = builder.Configuration.GetConnectionString("pgSql");
ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQueryService<EntryDto, Employeer?>, AutorizationQueryService>();
builder.Services.AddScoped<IQueryService<Employeer, ClaimsPrincipal>, CreatePrincipalQueryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "СКУД НАТК API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
