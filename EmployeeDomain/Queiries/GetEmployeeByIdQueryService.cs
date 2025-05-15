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
    public class GetEmployeeByIdQueryService
        : IQueryService<GetEmployeeByIdQuery, EmployeeDto>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdQueryService(IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public EmployeeDto Execute(GetEmployeeByIdQuery query)
        {
            return _repository.GetByIdAsync(query.Id)
                .GetAwaiter()
                .GetResult();
        }
    }
}
