using AutorizationDomain;
using AutorizationDomain.Queries;
using AutorizationDomain.Queries.Object;
using Data;
using Microsoft.EntityFrameworkCore;
using serviceSKUD;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string? path = builder.Configuration.GetConnectionString("pgSql");
ArgumentNullException.ThrowIfNull(path);
builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(path));

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQueryService<EntryDto, Employeer?>, AutorizationQueryService>();
builder.Services.AddScoped<IQueryService<Employeer, ClaimsPrincipal>, CreatePrincipalQueryService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();