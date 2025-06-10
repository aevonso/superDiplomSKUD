using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Hubs;
using Data;
using Data.Tables;
using AutorizationDomain.Queries.Object;
using Data.Tables.Data.Tables;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly Connection _dbContext;
        private readonly IHubContext<LogHub> _logHub;

        public AuthController(Connection dbContext, IHubContext<LogHub> logHub)
        {
            _dbContext = dbContext;
            _logHub = logHub;
        }

        // DTO ответа
        public class LoginResponse
        {
            public string Message { get; set; } = default!;
            public string? Token { get; set; }
        }

        // POST auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] EntryDto dto)
        {
            // 1) Проверяем тело запроса
            if (dto == null
                || string.IsNullOrWhiteSpace(dto.Login)
                || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Message = "Логин и пароль обязательны."
                });
            }

            // 2) Ищем пользователя
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == dto.Login);

            // 3) Неверные данные
            if (employee == null || employee.Password != dto.Password)
            {
                await LogFailedAttempt(dto.Login);
                return Unauthorized(new LoginResponse
                {
                    Message = "Неверный логин или пароль."
                });
            }

            // 4) Успешная авторизация
            await LogSuccessfulAttempt(employee);
            var token = GenerateToken(employee);

            return Ok(new LoginResponse
            {
                Message = "Вход выполнен успешно.",
                Token = token
            });
        }

        private async Task LogFailedAttempt(string login)
        {
            var log = new AccessAttempt
            {
                EmployeeId = 0,
                PointOfPassageId = null,
                Timestamp = DateTime.UtcNow,
                Success = false,
                FailureReason = "Неверный логин или пароль",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };
            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeFullName = "Неизвестный пользователь",
                pointName = "Авторизация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason
            });
        }

        private async Task LogSuccessfulAttempt(Employee employee)
        {
            var log = new AccessAttempt
            {
                EmployeeId = employee.Id,
                PointOfPassageId = null,
                Timestamp = DateTime.UtcNow,
                Success = true,
                FailureReason = null,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };
            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeFullName = $"{employee.LastName} {employee.FirstName}",
                pointName = "Авторизация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason
            });
        }

        // Простая генерация токена (можно заменить на JWT)
        private string GenerateToken(Employee employee)
            => $"{employee.Id}-{Guid.NewGuid()}";
    }
}
