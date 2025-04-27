using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries.Object
{
    public class AttemptDto
    {
        public string EmployeeFullName { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public string PointName { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
    }
}
