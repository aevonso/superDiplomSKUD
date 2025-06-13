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
                                          .Include(a => a.PointOfPassage).ThenInclude(p => p.Room)
                                          .AsQueryable();
            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);
            if (pointId.HasValue)
                query = query.Where(a => a.PointOfPassageId == pointId.Value);
            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            var efList = await query
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .ToListAsync();

            return efList.Select(a => new AttemptDto
            {
                EmployeeFullName = $"{a.Employee.LastName} {a.Employee.FirstName[0]}.",
                RoomName = a.PointOfPassage.Room.Name,
                PointName = a.PointOfPassage.Name,
                IpAddress = a.PointOfPassage.IpAddress,
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
                // Проверка на null для связанных объектов
                EmployeeFullName = a.Employee != null
                                   ? $"{a.Employee.LastName} {a.Employee.FirstName[0]}."
                                   : "Неизвестно",  // Если Employee == null, ставим заглушку
                RoomName = a.PointOfPassage?.Room?.Name ?? "Неизвестная комната",  // Используем null-оператор для проверки
                PointName = a.PointOfPassage?.Name ?? "Неизвестная точка",  // Используем null-оператор
                IpAddress = a.PointOfPassage?.IpAddress ?? "Неизвестный IP",  // Используем null-оператор
                Timestamp = a.Timestamp,
                Success = a.Success
            }).ToList();
        }
    }
}
