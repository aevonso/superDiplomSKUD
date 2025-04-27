using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries
{
    public class GetRecentAttemptsQuery: IQuery
    {
        public int Take { get; }
        public GetRecentAttemptsQuery(int take=10)
        {
            Take = take;
        }
    }
}
