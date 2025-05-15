using EmployeeDomain.IRepository;
using EmployeeDomain.Queiries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Queiries
{
    public class GetEmployeesQueryService
         : IQueryService<GetEmployeesQuery, IEnumerable<EmployeeDto>>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeesQueryService(IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<EmployeeDto> Execute(GetEmployeesQuery query)
        {
            return _repository.GetFilteredEmployeesAsync(query.Filter)
                .GetAwaiter()
                .GetResult();
        }
    }
}
