using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using serverSKUD.Hubs;
using Data;
using Data.Tables;
using System.Threading.Tasks;
using System;
using AutorizationDomain.Queries.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using serviceSKUD;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IQueryService<EntryDto, AuthResult> _loginService;
        private readonly IQueryService<RefreshDto, AuthResult> _refreshService;
        private readonly Connection _dbContext;
        private readonly IHubContext<LogHub> _logHub;

        public AuthController(
            IQueryService<EntryDto, AuthResult> loginService,
            IQueryService<RefreshDto, AuthResult> refreshService,
            Connection dbContext,
            IHubContext<LogHub> logHub)
        {
            _loginService = loginService;
            _refreshService = refreshService;
            _dbContext = dbContext;
            _logHub = logHub;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] EntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = _loginService.Execute(dto);
            bool success = authResult != null;

            // Получаем сотрудника, если есть
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == dto.Login);

            // Записываем лог попытки входа
            var log = new AccessAttempt
            {
                EmployeeId = employee?.Id ?? 0,
                PointOfPassageId = null, // null вместо 0
                Timestamp = DateTime.UtcNow,
                Success = success,
                FailureReason = success ? null : "Неверный логин или пароль",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };
            if (log.IpAddress == "::1")
            {
                log.IpAddress = "127.0.0.1";
            }
            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            // Отправляем лог в SignalR
            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeFullName = employee != null ? $"{employee.LastName} {employee.FirstName}" : "Неизвестный пользователь",
                pointName = "Авторизация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason
            });

            if (!success)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            return Ok(authResult);
        }
        [HttpPost("refresh")]
        public ActionResult<AuthResult> Refresh([FromBody] RefreshDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Не указан refreshToken" });

            var authResult = _refreshService.Execute(dto);
            if (authResult == null)
                return Unauthorized(new { message = "Неверный или просроченный refreshToken" });

            return Ok(authResult);
        }
    }
}
