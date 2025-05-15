using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries.Object
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int DivisionId { get; set; }
        public int PostId { get; set; }
        public string Email { get; set; } = null!;

    }
}
