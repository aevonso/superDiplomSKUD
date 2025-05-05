using DashboardDomain.Queries;
using DashboardDomain.Queries.Object;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IQueryService<ReportCriteria, ReportResult> _report;
        public ReportsController(IGenerateReportService report)
        {
            _report = report;
        }

        [HttpPost]
        public IActionResult Generate([FromBody]ReportCriteria c)
        {
            var r = _report.Execute(c);
            return File(r.Content, r.MimeType, r.FileName);
        }
    }
}
