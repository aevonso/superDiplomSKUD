using System;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Hubs;
using serverSKUD.Model;

[ApiController]
[Route("api/[controller]")]
public class QrAccessController : ControllerBase
{
    private readonly Connection _db;
    private readonly IHubContext<LogHub> _logHub;

    public QrAccessController(Connection db, IHubContext<LogHub> logHub)
    {
        _db = db;
        _logHub = logHub;
    }

    // --------------------------------------------------------------------
    // 1.  Проверка доступа по QR-коду + логирование попытки
    // --------------------------------------------------------------------
    [HttpPost("check")]
    public async Task<IActionResult> CheckAccess([FromBody] QrAccessDto dto)
    {
        // 1)  Ищем запись в матрице доступа
        bool hasAccess = await _db.AccessMatrices.AnyAsync(m =>
            m.PostId == dto.PostId &&
            m.RoomId == dto.RoomId &&
            m.IsAccess);

        // 2)  Логируем попытку
        await LogQrAttempt(dto, hasAccess);

        // 3)  Отдаём результат клиенту
        return Ok(new { hasAccess });
    }

    // --------------------------------------------------------------------
    // 2.  GET-энд-пойнт — список должностей + признак доступа
    // --------------------------------------------------------------------
    [HttpGet("/api/Post/withAccess")]
    public async Task<IActionResult> GetPostsWithAccess([FromQuery] int roomId)
    {
        var posts = await _db.Posts
            .Include(p => p.Division)
            .Select(p => new
            {
                p.Id,
                Name = p.Name,
                Division = p.Division.Name,
                HasAccess = _db.AccessMatrices.Any(m =>
                    m.PostId == p.Id &&
                    m.RoomId == roomId &&
                    m.IsAccess)
            })
            .ToListAsync();

        return Ok(posts);
    }

    // --------------------------------------------------------------------
    // 3.  POST — валидация кода мобильного устройства
    // --------------------------------------------------------------------
    [HttpPost("validate")]
    public async Task<ActionResult<ValidateCodeResponse>> Validate([FromBody] ValidateCodeRequest dto)
    {
        bool isValid = await _db.MobileDevices.AnyAsync(d =>
            d.DeviceCode == dto.DeviceCode && d.IsActive);

        return Ok(new ValidateCodeResponse { IsValid = isValid });
    }

    // --------------------------------------------------------------------
    // 4.  Приватный метод логирования
    // --------------------------------------------------------------------
    private async Task LogQrAttempt(QrAccessDto dto, bool success)
    {
        var post = await _db.Posts.FindAsync(dto.PostId);
        var room = await _db.Rooms.FindAsync(dto.RoomId);

        string employeeFullName = "Неизвестный пользователь";
        int? employeeId = null;

        // Логируем полученный EmployeeId
        Console.WriteLine($"Получен EmployeeId: {dto.EmployeeId}");

        if (dto.EmployeeId > 0)
        {
            var employee = await _db.Employees.FindAsync(dto.EmployeeId);
            if (employee != null)
            {
                employeeFullName = $"{employee.LastName} {employee.FirstName}";
                employeeId = employee.Id;
                Console.WriteLine($"Найден сотрудник: {employeeFullName} с ID: {employeeId}");
            }
            else
            {
                Console.WriteLine($"Сотрудник с EmployeeId {dto.EmployeeId} не найден в базе данных.");
            }
        }
        else
        {
            Console.WriteLine("EmployeeId не передан или равен нулю.");
        }

        var attempt = new AccessAttempt
        {
            EmployeeId = employeeId,
            PointOfPassageId = null,
            Timestamp = DateTime.UtcNow,
            Success = success,
            FailureReason = success ? null : "Доступ запрещён",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
        };

        _db.AccessAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        // Логируем результат
        Console.WriteLine($"Логирование попытки доступа для сотрудника: {employeeFullName}");

        await _logHub.Clients.All.SendAsync("ReceiveLog", new
        {
            timestamp = attempt.Timestamp,
            employeeFullName,
            roomName = room?.Name ?? $"Room {dto.RoomId}",
            pointName = "QR-сканер",
            ipAddress = attempt.IpAddress,
            success = attempt.Success,
            failureReason = attempt.FailureReason
        });
    }



}

// DTO для запроса/ответа
public class ValidateCodeRequest
{
    public string DeviceCode { get; set; } = default!;
}

public class ValidateCodeResponse
{
    public bool IsValid { get; set; }
}

public class QrAccessDto
{
    public int PostId { get; set; }
    public int RoomId { get; set; }
    public int EmployeeId { get; set; }
}
