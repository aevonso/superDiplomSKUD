using Data.Tables;
using Data.Tables.Data.Tables;
using EmployeeDomain.IRepository;
using EmployeeDomain.Queiries.Object;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class EmployeePageRepository : IEmployeeRepository
    {
        private readonly Connection _db;

        public EmployeePageRepository(Connection db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Read operations
        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            return await _db.Employees
                .Where(e => e.Id == id)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}",
                    PhoneNumber = FormatPhoneNumber(e.PhoneNumber),
                    Login = e.Login,
                    Division = e.Division.Name,
                    Position = e.Post.Name,
                    Email = e.Email
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EmployeeDto>> GetFilteredEmployeesAsync(EmployeeFilter filter)
        {
            var query = _db.Employees
                .Include(e => e.Division)
                .Include(e => e.Post)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(e =>
                    (e.LastName + " " + e.FirstName + " " + e.Patronymic)
                    .Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.Phone))
            {
                query = query.Where(e => e.PhoneNumber.Contains(filter.Phone));
            }

            if (!string.IsNullOrEmpty(filter.Login))
            {
                query = query.Where(e => e.Login.Contains(filter.Login));
            }

            if (!string.IsNullOrEmpty(filter.Position))
            {
                query = query.Where(e => e.Post.Name.Contains(filter.Position));
            }

            return await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Take(filter.Take)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}",
                    PhoneNumber = FormatPhoneNumber(e.PhoneNumber),
                    Login = e.Login,
                    Division = e.Division.Name,
                    Position = e.Post.Name,
                    Email = e.Email
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountAsync(EmployeeFilter filter)
        {
            var query = _db.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(e =>
                    (e.LastName + " " + e.FirstName + " " + e.Patronymic)
                    .Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.Position))
            {
                query = query.Include(e => e.Post)
                    .Where(e => e.Post.Name.Contains(filter.Position));
            }

            return await query.CountAsync();
        }

        // Create operation
        public async Task<int> CreateAsync(EmployeeCreateDto dto)
        {
            // Validate division exists
            var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == dto.DivisionId);
            if (!divisionExists)
                throw new ArgumentException("Division not found");

            // Validate post exists
            var postExists = await _db.Posts.AnyAsync(p => p.Id == dto.PostId);
            if (!postExists)
                throw new ArgumentException("Post not found");

            // Check for duplicate login
            var loginExists = await _db.Employees.AnyAsync(e => e.Login == dto.Login);
            if (loginExists)
                throw new ArgumentException("Login already exists");

            var employee = new Employee
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Patronymic = dto.Patronymic,
                PhoneNumber = NormalizePhoneNumber(dto.PhoneNumber),
                Login = dto.Login,
                DivisionId = dto.DivisionId,
                PostId = dto.PostId,
                Email = dto.Email,
                PassportNumber = dto.PassportNumber,
                PassportSeria = dto.PassportSeria,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Employees.AddAsync(employee);
            await _db.SaveChangesAsync();

            return employee.Id;
        }

        // Update operation
        public async Task UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _db.Employees.FindAsync(dto.Id);
            if (employee == null)
                throw new ArgumentException("Employee not found");

            // Validate division exists if changed
            if (employee.DivisionId != dto.DivisionId)
            {
                var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == dto.DivisionId);
                if (!divisionExists)
                    throw new ArgumentException("Division not found");
            }

            // Validate post exists if changed
            if (employee.PostId != dto.PostId)
            {
                var postExists = await _db.Posts.AnyAsync(p => p.Id == dto.PostId);
                if (!postExists)
                    throw new ArgumentException("Post not found");
            }

            employee.LastName = dto.LastName;
            employee.FirstName = dto.FirstName;
            employee.Patronymic = dto.Patronymic;
            employee.PhoneNumber = NormalizePhoneNumber(dto.PhoneNumber);
            employee.DivisionId = dto.DivisionId;
            employee.PostId = dto.PostId;
            employee.Email = dto.Email;

            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }

        // Delete operation
        public async Task DeleteAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
                throw new ArgumentException("Employee not found");

            // Check if employee has related records
            var hasAttempts = await _db.AccessAttempts.AnyAsync(a => a.EmployeeId == id);
            if (hasAttempts)
                throw new InvalidOperationException("Cannot delete employee with related access attempts");

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }

        // Helper methods
        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            return phone.Length == 11 ?
                $"+7 ({phone[1..4]}) {phone[4..7]}-{phone[7..9]}-{phone[9..]}" :
                phone;
        }

        private string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;

            // Remove all non-digit characters
            return new string(phone.Where(char.IsDigit).ToArray());
        }

        // Optional: Password update method
      
        
    }
}