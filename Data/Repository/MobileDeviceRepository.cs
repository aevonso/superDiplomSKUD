using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using Data;                        // ваш DbContext-неймспейс
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class MobileDeviceRepository : IMobileDeviceRepository
    {
        private readonly Connection _db;
        public MobileDeviceRepository(Connection db) => _db = db;

        public Task<int> CountAsync(bool onlyActive)
        {
            var q = _db.MobileDevices.AsQueryable();
            if (onlyActive)
                q = q.Where(d => d.IsActive);
            return q.CountAsync();
        }

        public Task<List<MobileDeviceDto>> GetAllAsync(bool onlyActive)
        {
            var q = _db.MobileDevices.AsNoTracking()
                       .Include(d => d.Employee)
                       .AsQueryable();
            if (onlyActive) q = q.Where(d => d.IsActive);

            return q
              .Select(d => new MobileDeviceDto
              {
                  Id = d.Id,
                  EmployeeId = d.EmployerId,
                  IsActive = d.IsActive,
                  CreatedAt = d.CreatedAt
              })
              .ToListAsync();
        }
    }
}
