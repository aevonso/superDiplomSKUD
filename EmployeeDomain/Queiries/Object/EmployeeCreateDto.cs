using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries.Object
{
    public class EmployeeCreateDto
    {
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int DivisionId { get; set; }
        public int PostId { get; set; }

        public string PassportNumber { get; set; } = null!;
        public string PassportSeria { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
