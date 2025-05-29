using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using serverSKUD.Hubs;
using Data.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Data;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly Connection _dbContext;
        private readonly IHubContext<LogHub> _logHub;

        public DashboardController(Connection dbContext, IHubContext<LogHub> logHub)
        {
            _dbContext = dbContext;
            _logHub = logHub;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var employeesCount = await _dbContext.Employees.CountAsync();
            var devicesCount = await _dbContext.MobileDevices.CountAsync();
            return Ok(new { employeesCount, devicesCount });
        }

        [HttpGet("attempts")]
        public IActionResult GetAttempts(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? pointId,
            [FromQuery] int? employeeId,
            [FromQuery] int take = 10)
        {
            var query = _dbContext.AccessAttempts.AsQueryable();

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);
            if (pointId.HasValue)
                query = query.Where(a => a.PointOfPassageId == pointId.Value);
            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            var attempts = query
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .Select(a => new {
                    timestamp = a.Timestamp,
                    employeeFullName = a.Employee.LastName + " " + a.Employee.FirstName,
                    pointName = a.PointOfPassage != null ? a.PointOfPassage.Name : "Нет точки",
                    ipAddress = a.IpAddress,
                    success = a.Success
                })
                .ToList();

            return Ok(attempts);
        }

        // Принимает новые логи и рассылает всем клиентам через SignalR
        [HttpPost("log")]
        public async Task<IActionResult> PostLog([FromBody] AccessAttemptDto dto)
        {
            var attempt = new AccessAttempt
            {
                EmployeeId = dto.EmployeeId,
                PointOfPassageId = dto.PointOfPassageId == 0 ? null : dto.PointOfPassageId,
                Timestamp = dto.Timestamp,
                Success = dto.Success,
                FailureReason = dto.FailureReason,
                IpAddress = dto.IpAddress
            };
            if (attempt.IpAddress == "::1")
            {
                attempt.IpAddress = "127.0.0.1";
            }
            _dbContext.AccessAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            var employee = await _dbContext.Employees.FindAsync(attempt.EmployeeId);
            string employeeFullName = employee != null ? $"{employee.LastName} {employee.FirstName}" : "Неизвестный сотрудник";

            string pointName = "Нет точки";
            if (attempt.PointOfPassageId != 0)
            {
                var point = await _dbContext.PointOfPassages.FindAsync(attempt.PointOfPassageId);
                pointName = point != null ? point.Name : "Неизвестная точка";
            }
            else
            {
                pointName = "Нет точки";
            }

            var broadcastDto = new
            {
                timestamp = attempt.Timestamp,
                employeeFullName = employeeFullName,
                pointName = pointName,
                ipAddress = attempt.IpAddress,
                success = attempt.Success,
                failureReason = attempt.FailureReason
            };

            await _logHub.Clients.All.SendAsync("ReceiveLog", broadcastDto);

            return Ok();
        }
    }

    public class AccessAttemptDto
    {
        public int EmployeeId { get; set; }
        public int? PointOfPassageId { get; set; }  // nullable
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
        public string IpAddress { get; set; } = null!;
    }
}
