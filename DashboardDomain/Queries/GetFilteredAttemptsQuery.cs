using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries
{
    public class GetFilteredAttemptsQuery : IQuery
    {
        public DateTime? From { get; }
        public DateTime? To { get; }
        public int? PointId { get; }
        public int? EmployeeId { get; }
        public int Take { get; }

        public GetFilteredAttemptsQuery(DateTime? from = null, 
            DateTime? to = null, 
            int? pointId = null, 
            int? employeeId = null, 
            int take = 10)
        {
            From = from;
            To = to;
            PointId = pointId;
            EmployeeId = employeeId;
            Take = take;
        }
    }
}
