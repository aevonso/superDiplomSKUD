using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries.Object
{
    public class EmployeeFilter
    {
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Position { get; set; } = null!;
        public int Take { get; set; } = 10;
    }
}
