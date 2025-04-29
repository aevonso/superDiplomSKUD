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

        //метод для фильтров
        Task<List<AttemptDto>> GetFilteredAttemptsAsync(
            DateTime? from,
            DateTime? to,
            int? pointId,
            int? employeeId,
            int take
            );

        //метод для обшего кол-ва попыток(без фильтра)
        Task<int> CountAttemptAsync();

    }
}
