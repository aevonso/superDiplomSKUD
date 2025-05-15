using EmployeeDomain.IRepository;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries
{
    public class GetEmployeesCountQueryService
        : IQueryService<GetEmployeesCountQuery, int>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeesCountQueryService(IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public int Execute(GetEmployeesCountQuery query)
        {
            return _repository.CountAsync(query.Filter)
                .GetAwaiter()
                .GetResult();
        }
    }
}
