using DashboardDomain.IRepository;
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
    }
}
