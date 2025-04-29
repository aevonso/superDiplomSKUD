using AuntificationDomain;
using Data.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class TwoFaRepository : I2FaRepository
    {
        private readonly Connection _connection;

        public TwoFaRepository(Connection connection)
        {
            _connection = connection
                ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<string> GenerateCodeAsync(string login)
        {
            var emp = await _connection.Employees
                                      .AsNoTracking()
                                      .SingleOrDefaultAsync(e => e.Login == login);
            if (emp == null)
                throw new InvalidOperationException($"Пользователь '{login}' не найден");

            var code = new Random().Next(0, 1_000_000).ToString("D6");

            var twoFa = new TwoFactorCode
            {
                EmployeeId = emp.Id,
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _connection.twoFactorCodes.Add(twoFa);
            await _connection.SaveChangesAsync();

            return code;
        }

        public async Task<string?> GetEmailByLoginAsync(string login)
        {
            var emp = await _connection.Employees
                         .AsNoTracking()
                         .SingleOrDefaultAsync(e => e.Login == login);
            return emp?.Email;
        }

        public async Task<bool> ValidateCodeAsync(string login, string code)
        {
            var entry = await _connection.twoFactorCodes
                .Include(t => t.Employee)
                .Where(t => !t.IsUsed
                         && t.Expiry >= DateTime.UtcNow
                         && t.Employee.Login == login)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(t => t.Code == code);

            if (entry == null)
                return false;

            entry.IsUsed = true;
            await _connection.SaveChangesAsync();
            return true;
        }
    }
}
