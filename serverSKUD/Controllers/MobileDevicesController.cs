using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MobileDevicesController : ControllerBase
    {
        private readonly Connection _db;

        public MobileDevicesController(Connection db)
        {
            _db = db;
        }

        // GET с фильтрами
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobileDeviceDto>>> GetAll([FromQuery] MobileDeviceFilter filter)
        {
            var query = _db.MobileDevices
                .Include(d => d.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.DeviceName))
                query = query.Where(d => d.DeviceName.ToLower().Contains(filter.DeviceName.ToLower()));

            if (filter.EmployerId.HasValue)
                query = query.Where(d => d.EmployerId == filter.EmployerId);

            if (filter.DateFrom.HasValue)
                query = query.Where(d => d.CreatedAt >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(d => d.CreatedAt <= filter.DateTo.Value);

            var list = await query
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new MobileDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    DeviceCode = d.DeviceCode,
                    CreatedAt = d.CreatedAt,
                    IsActive = d.IsActive,
                    EmployeeName =
                        d.Employee.LastName + " " +
                        // первая буква имени
                        d.Employee.FirstName.Substring(0, 1) + "." +
                        // если есть отчество — первая буква, иначе пусто
                        (d.Employee.Patronymic != null && d.Employee.Patronymic.Length > 0
                            ? d.Employee.Patronymic.Substring(0, 1)
                            : String.Empty)
                })
                .ToListAsync();

            return Ok(list);
        }
        [HttpPost("validate")]
        public async Task<ActionResult<ValidateCodeResponse>> Validate([FromBody] ValidateCodeRequest req)
        {
            bool exists = await _db.MobileDevices
                .AnyAsync(d => d.DeviceCode == req.DeviceCode);

            return Ok(new ValidateCodeResponse { IsValid = exists });
        }
        // GET по ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MobileDeviceDto>> GetById(int id)
        {
            var d = await _db.MobileDevices
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d == null) return NotFound();

            return Ok(new MobileDeviceDto
            {
                Id = d.Id,
                DeviceName = d.DeviceName,
                DeviceCode = d.DeviceCode,
                CreatedAt = d.CreatedAt,
                IsActive = d.IsActive,
                EmployeeName =
                    d.Employee.LastName + " " +
                    d.Employee.FirstName.Substring(0, 1) + "." +
                    (d.Employee.Patronymic != null && d.Employee.Patronymic.Length > 0
                        ? d.Employee.Patronymic.Substring(0, 1)
                        : String.Empty)
            });
        }

        // POST — Создание
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateMobileDeviceDto dto)
        {
            var deviceCode = GenerateDeviceCode();

            var device = new MobileDevice
            {
                EmployerId = dto.EmployerId,
                DeviceName = dto.DeviceName,
                DeviceCode = deviceCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.MobileDevices.Add(device);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = device.Id }, device.Id);
        }

        // PUT — Обновление
        [HttpPut("{id:int}")]
        public async Task<ActionResult<string>> Update(int id, [FromBody] UpdateMobileDeviceDto dto)
        {
            var d = await _db.MobileDevices.FindAsync(id);
            if (d == null) return NotFound();

            d.DeviceName = dto.DeviceName;
            d.DeviceCode = GenerateDeviceCode();
            await _db.SaveChangesAsync();

            return Ok(d.DeviceCode);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.MobileDevices.FindAsync(id);
            if (d == null) return NotFound();

            _db.MobileDevices.Remove(d);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private string GenerateDeviceCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
