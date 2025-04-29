using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.IRepository
{
    public interface IEmployeeRepository
    {
        Task<int> CountAsync();
    }
}
