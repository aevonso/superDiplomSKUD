using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries
{
    public class GetDashboardStatsQueryService : IQueryService<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IEmployeeRepository _empRep;
        private readonly IMobileDeviceRepository _mobRep;
        private readonly IAccessAttemptRepository _attRep;

        public GetDashboardStatsQueryService(IEmployeeRepository empRep, IMobileDeviceRepository mobRep, IAccessAttemptRepository attRep)
        {
            _empRep = empRep;
            _mobRep = mobRep;
            _attRep = attRep;
        }

        public DashboardStatsDto Execute(GetDashboardStatsQuery obj)
        {
            var empCount = _empRep.CountAsync().GetAwaiter().GetResult();
            var mobCount = _mobRep.CountAsync(onlyActive: true).GetAwaiter().GetResult();
            var attCount = _attRep.CountAttemptAsync().GetAwaiter().GetResult();

            return new DashboardStatsDto
            {
                EmployeeCount = empCount,
                MobileDeviceCount = mobCount,
                AttemptCount = attCount
            };
        }
    }
}
