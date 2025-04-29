using DashboardDomain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly Connection _db;
        public EmployeeRepository(Connection db) => _db = db;
        public Task<int> CountAsync() => _db.Employees.CountAsync();
    }
}
