using DashboardDomain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashboardDomain.Queries.Object;

namespace Data.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly Connection _db;
        public EmployeeRepository(Connection db) => _db = db;
        public Task<int> CountAsync() => _db.Employees.CountAsync();

        public Task<List<EmployeeDto>> GetAllAsync()
        => _db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                LastName = e.LastName,
                FirstName = e.FirstName,
                Email = e.Email

            }).ToListAsync();
    }
}
