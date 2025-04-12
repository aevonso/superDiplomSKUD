using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries.Object
{
    public class QrInfo
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string QrBase64 { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
