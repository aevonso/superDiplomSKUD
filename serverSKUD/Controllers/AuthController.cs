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

        public class LoginResponse
        {
            public string Message { get; set; } = default!;
            public string? Token { get; set; }
            public int? EmployeeId { get; set; }  
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] EntryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password))
            {
                await LogFailedAttempt(null, dto?.Login ?? "Unknown", "Не указаны логин или пароль");
                return BadRequest(new LoginResponse { Message = "Логин и пароль обязательны." });
            }

            // Находим сотрудника по логину (даже если пароль неверный)
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == dto.Login);

            if (employee == null || employee.Password != dto.Password)
            {
                await LogFailedAttempt(employee?.Id, dto.Login, "Неверный логин или пароль");
                return Unauthorized(new LoginResponse
                {
                    Message = "Неверный логин или пароль.",
                    EmployeeId = employee?.Id  // Возвращаем ID, даже если аутентификация не удалась
                });
            }

            await LogSuccessfulAttempt(employee);
            var token = GenerateToken(employee);

            return Ok(new LoginResponse
            {
                Message = "Вход выполнен успешно.",
                Token = token,
                EmployeeId = employee.Id  // ID сотрудника при успешной аутентификации
            });
        }

        private async Task LogFailedAttempt(int? employeeId, string login, string reason)
        {
            string employeeName = "Неизвестный пользователь";
            if (employeeId.HasValue)
            {
                var employee = await _dbContext.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == employeeId);
                if (employee != null)
                {
                    employeeName = $"{employee.LastName} {employee.FirstName}";
                }
            }

            var log = new AccessAttempt
            {
                EmployeeId = employeeId,
                PointOfPassageId = null,
                Timestamp = DateTime.UtcNow,
                Success = false,
                FailureReason = reason,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            };

            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeId = employeeId,
                employeeFullName = employeeName,
                pointName = "Авторизация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason,
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
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            };

            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeId = employee.Id,
                employeeFullName = $"{employee.LastName} {employee.FirstName}",
                pointName = "Авторизация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason,
            });
        }

        private string GenerateToken(Employee employee) => $"{employee.Id}-{Guid.NewGuid()}";
    }
}