using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using Data.Tables.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Model;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly Connection _db;

        public EmployeeController(Connection db)
        {
            _db = db;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll(
            [FromQuery] string? fullName,
            [FromQuery] string? phone,
            [FromQuery] string? login,
            [FromQuery] int? divisionId,
            [FromQuery] int? postId,
            [FromQuery] int take = 100)
        {
            IQueryable<Employee> q = _db.Employees
                .Include(e => e.Division)
                .Include(e => e.Post);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var fn = fullName.Trim().ToLower();
                q = q.Where(e =>
                    e.LastName.ToLower().Contains(fn) ||
                    e.FirstName.ToLower().Contains(fn) ||
                    e.Patronymic.ToLower().Contains(fn));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var ph = phone.Trim();
                q = q.Where(e => e.PhoneNumber.Contains(ph));
            }

            if (!string.IsNullOrWhiteSpace(login))
            {
                var lg = login.Trim().ToLower();
                q = q.Where(e => e.Login.ToLower().Contains(lg));
            }

            if (divisionId.HasValue)
                q = q.Where(e => e.DivisionId == divisionId.Value);

            if (postId.HasValue)
                q = q.Where(e => e.PostId == postId.Value);

            var list = await q
                .Take(take)
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/Employee/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var emp = await _db.Employees
                .Include(e => e.Division)
                .Include(e => e.Post)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<ActionResult<Employee>> Create([FromBody] EmployeeCreateDto dto)
        {
            var emp = new Employee
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Patronymic = dto.Patronymic,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Login = dto.Login,
                Password = dto.Password,
                DivisionId = dto.DivisionId,
                PostId = dto.PostId,
                PassportSeria = dto.PassportSeria,
                PassportNumber = dto.PassportNumber,
                Avatar = null
            };

            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                                   new { id = emp.Id },
                                   emp);
        }

        // PUT: api/Employee/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            // копируем поля из dto
            emp.LastName = dto.LastName;
            emp.FirstName = dto.FirstName;
            emp.Patronymic = dto.Patronymic;
            emp.Email = dto.Email;
            emp.PhoneNumber = dto.PhoneNumber;
            emp.DivisionId = dto.DivisionId;
            emp.PostId = dto.PostId;
            emp.PassportSeria = dto.PassportSeria;
            emp.PassportNumber = dto.PassportNumber;

            _db.Entry(emp).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Employee/5/avatar
        [HttpGet("{id:int}/avatar")]
        public async Task<IActionResult> GetAvatar(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp?.Avatar == null || emp.Avatar.Length == 0)
                return NotFound();

            // Отдаём как PNG
            return File(emp.Avatar, "image/png");
        }

        // PUT: api/Employee/5/avatar
        [HttpPut("{id:int}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            emp.Avatar = ms.ToArray();

            _db.Entry(emp).Property(e => e.Avatar).IsModified = true;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Employee/5/avatar
        [HttpDelete("{id:int}/avatar")]
        public async Task<IActionResult> DeleteAvatar(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            emp.Avatar = null;
            _db.Entry(emp).Property(e => e.Avatar).IsModified = true;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
