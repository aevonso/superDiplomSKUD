using EmployeeDomain.Queiries.Object;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeDomain.IRepository
{
    public interface IEmployeeRepository
    {
        // Чтение
        Task<EmployeeDto> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeDto>> GetFilteredEmployeesAsync(EmployeeFilter filter);
        Task<int> CountAsync(EmployeeFilter filter);

        // Создание, обновление, удаление
        Task<int> CreateAsync(EmployeeCreateDto dto);
        Task UpdateAsync(EmployeeUpdateDto dto);
        Task DeleteAsync(int id);
    }
}