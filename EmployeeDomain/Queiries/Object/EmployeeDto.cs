using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries.Object
{
    public class EmployeeDto
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Division { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
