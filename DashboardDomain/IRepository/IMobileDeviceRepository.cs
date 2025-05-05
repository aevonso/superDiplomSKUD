using DashboardDomain.Queries.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.IRepository
{
    public interface IMobileDeviceRepository
    {
        //число активных мобилок 
        Task<int> CountAsync(bool onlyActive);
        Task<List<MobileDeviceDto>> GetAllAsync(bool onlyActive);
    }
}
