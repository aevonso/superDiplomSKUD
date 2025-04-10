using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries.Object
{
    public class Employeer : IQuery
    {
        public string Login { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = null!;
    }
}
