using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class AccessAttemptRepository : IAccessAttemptRepository
    {
        private readonly Connection _db;
        public AccessAttemptRepository(Connection db) => _db = db;

        public Task<int> CountAttemptAsync() => _db.AccessAttempts.CountAsync();

        public async Task<List<AttemptDto>> GetFilteredAttemptsAsync(DateTime? from, DateTime? to, int? pointId, int? employeeId, int take)
        {
            var query = _db.AccessAttempts.AsNoTracking()
                            .Include(a => a.Employee)
                            .Include(a => a.PointOfPassage)
                                .ThenInclude(p => p.Room)
                            .AsQueryable();

            // Добавляем логирование параметров
            Console.WriteLine($"Filter params - From: {from}, To: {to}, PointId: {pointId}, EmployeeId: {employeeId}");

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value.Date); // Используем .Date для сравнения дат без времени

            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value.Date.AddDays(1).AddTicks(-1)); // Включаем весь последний день

            // Остальные условия фильтрации...

            var result = await query.OrderByDescending(a => a.Timestamp)
                                   .Take(take)
                                   .ToListAsync();

            Console.WriteLine($"Found {result.Count} records"); // Логируем количество найденных записей

            return result.Select(a => new AttemptDto
            {
                EmployeeFullName = a.Employee != null
                    ? $"{a.Employee.LastName} {a.Employee.FirstName[0]}."
                    : "Неизвестно",
                RoomName = a.PointOfPassage?.Room?.Name ?? "Неизвестная комната",
                PointName = a.PointOfPassage?.Name ?? "Мобильное устройство",
                IpAddress = FormatIpAddress(a.IpAddress), 
                Timestamp = a.Timestamp,
                Success = a.Success
            }).ToList();
        }

        public async Task<List<AttemptDto>> GetRecentAttemptsAsync(int take)
        {
            var efList = await _db.AccessAttempts
                                  .AsNoTracking()
                                  .Include(a => a.Employee)
                                  .Include(a => a.PointOfPassage)
                                      .ThenInclude(p => p.Room)
                                  .OrderByDescending(a => a.Timestamp)
                                  .Take(take)
                                  .ToListAsync();

            return efList.Select(a => new AttemptDto
            {
                EmployeeFullName = a.Employee != null
                                   ? $"{a.Employee.LastName} {a.Employee.FirstName[0]}."
                                   : "Неизвестно",
                RoomName = a.PointOfPassage?.Room?.Name ?? "Неизвестная комната",
                PointName = a.PointOfPassage?.Name ?? "Неизвестная точка",
                IpAddress = FormatIpAddress(a.IpAddress),
                Timestamp = a.Timestamp,
                Success = a.Success
            }).ToList();
        }


        private string FormatIpAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return "Неизвестный IP";

            // Удаляем IPv6 префикс для IPv4-адресов
            if (ipAddress.StartsWith("::ffff:"))
                return ipAddress.Substring(7); // Удаляем первые 7 символов

            // Преобразуем локальный IPv6 в IPv4
            if (ipAddress == "::1")
                return "127.0.0.1";

            return ipAddress;
        }
    }
}
