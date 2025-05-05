using DashboardDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries
{
    public interface IGenerateReportService : IQueryService<ReportCriteria, ReportResult>
    {
    }
}
