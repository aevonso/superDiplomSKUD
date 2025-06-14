using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries.Object
{
    public class ReportCriteria : IQuery
    {
        public bool IncludeEmployees { get; set; }
        public bool IncludeMobileDevices { get; set; }
        public bool IncludeAccessAttempts { get; set; }
        public string Format { get; set; } = "pdf"; // Формат отчета (pdf или docx)

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public bool SuccessOnly { get; set; }
        public bool FailedOnly { get; set; }
    }
}
