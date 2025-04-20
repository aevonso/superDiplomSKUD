using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries.Object
{
    public class RefreshDto : IQuery
    {
        public string RefreshToken { get; set; } = null!;
    }
}
