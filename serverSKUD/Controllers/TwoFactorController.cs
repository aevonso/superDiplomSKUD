using AuntificationDomain;
using AuntificationDomain.Queries.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using serverSKUD.Hubs;
using Data;
using Data.Tables;
using System.Threading.Tasks;
using serviceSKUD;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("auth/2fa")]
    [AllowAnonymous]
    public class TwoFactorController : ControllerBase
    {
        private readonly IQueryService<TwoFactorGenerateDto, Task<string>> _genSvc;
        private readonly IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>> _valSvc;
        private readonly IEmailSender _email;
        private readonly I2FaRepository _twoFaRepo;
        private readonly Connection _dbContext;
        private readonly IHubContext<LogHub> _logHub;

        public TwoFactorController(
            IQueryService<TwoFactorGenerateDto, Task<string>> genSvc,
            IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>> valSvc,
            IEmailSender emailSender,
            I2FaRepository twoFaRepo,
            Connection dbContext,
            IHubContext<LogHub> logHub)
        {
            _genSvc = genSvc;
            _valSvc = valSvc;
            _email = emailSender;
            _twoFaRepo = twoFaRepo;
            _dbContext = dbContext;
            _logHub = logHub;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] TwoFactorGenerateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await Log2FaAttempt(null, dto.Login, false, "Неверный формат запроса");
                return BadRequest(ModelState);
            }

            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == dto.Login);

            if (employee == null)
            {
                await Log2FaAttempt(null, dto.Login, false, "Пользователь не найден");
                return NotFound(new { message = "Пользователь не найден" });
            }

            var email = await _twoFaRepo.GetEmailByLoginAsync(dto.Login);
            if (string.IsNullOrEmpty(email))
            {
                await Log2FaAttempt(employee.Id, dto.Login, false, "Email пользователя не найден");
                return NotFound(new { message = "Email пользователя не найден" });
            }

            var code = await _genSvc.Execute(dto);
            var subject = "Ваш код 2FA для СКУД НАТК";
            var body = $"<p>Ваш код для входа: <b>{code}</b></p>";

            await _email.SendCodeAsync(email, subject, body);
            await Log2FaAttempt(employee.Id, dto.Login, true, "Код 2FA отправлен");

            return Ok(new
            {
                message = "Код отправлен на почту",
                userId = employee.Id // Добавляем ID пользователя в ответ
            });
        }

        [HttpPost("validate")]
        public async Task<ActionResult<TwoFactorResult>> Validate([FromBody] TwoFactorValidateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await Log2FaAttempt(null, dto.Login, false, "Неверный формат запроса");
                return BadRequest(ModelState);
            }

            // Находим сотрудника по логину
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == dto.Login);

            var result = await _valSvc.Execute(dto);

            if (!result.Success)
            {
                await Log2FaAttempt(employee?.Id, dto.Login, false, "Неверный код 2FA");
                return Unauthorized(result);
            }

            // Заполняем данные сотрудника в результате
            result.EmployeeId = employee?.Id;
            result.EmployeeFullName = employee != null ? $"{employee.LastName} {employee.FirstName}" : null;

            await Log2FaAttempt(employee?.Id, dto.Login, true, "Успешная аутентификация 2FA");
            return Ok(result);
        }

        private async Task Log2FaAttempt(int? employeeId, string login, bool success, string reason)
        {
            var employeeName = "Неизвестный пользователь";
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
                Success = success,
                FailureReason = success ? null : reason,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            };

            _dbContext.AccessAttempts.Add(log);
            await _dbContext.SaveChangesAsync();

            await _logHub.Clients.All.SendAsync("ReceiveLog", new
            {
                timestamp = log.Timestamp,
                employeeId = employeeId,
                employeeFullName = employeeName,
                pointName = "Двухфакторная аутентификация",
                ipAddress = log.IpAddress,
                success = log.Success,
                failureReason = log.FailureReason,
            });
        }
    }
}