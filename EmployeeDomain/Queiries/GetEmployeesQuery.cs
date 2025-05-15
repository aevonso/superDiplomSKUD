using EmployeeDomain.Queiries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries
{
    public class GetEmployeesQuery : IQuery
    {
        public EmployeeFilter Filter { get; }

        public GetEmployeesQuery(EmployeeFilter filter)
        {
            Filter = filter;
        }
    }
}
