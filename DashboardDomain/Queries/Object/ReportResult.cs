using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries.Object
{
    public class ReportResult
    {
        public byte[] Content { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
