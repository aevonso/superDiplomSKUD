using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardDomain.Queries.Object;

namespace DashboardDomain.IRepository
{
    public interface IAccessAttemptRepository
    {
        Task<List<AttemptDto>> GetRecentAttemptsAsync(int take);

    }
}
