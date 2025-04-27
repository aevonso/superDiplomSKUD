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

        public async Task<List<AttemptDto>> GetRecentAttemptsAsync(int take)
        {
            var efList = await _db.AccessAttempts
                                  .AsNoTracking()
                                  .Include(a => a.Employee)
                                  .Include(a => a.PointOfPassage).ThenInclude(p => p.Room)
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
    }
}
