using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries.Object
{
    public class DashboardStatsDto
    {
        public int EmployeeCount { get; set; }
        public int MobileDeviceCount { get; set; }
        public int AttemptCount { get; set; }
    }
}
