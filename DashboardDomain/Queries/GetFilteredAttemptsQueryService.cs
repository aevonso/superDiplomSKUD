using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardDomain.Queries
{
    public class GetFilteredAttemptsQueryService
    : IQueryService<GetFilteredAttemptsQuery, IEnumerable<AttemptDto>>
    {
        private readonly IAccessAttemptRepository _repo;
        public GetFilteredAttemptsQueryService(IAccessAttemptRepository repo) => _repo = repo;

        public IEnumerable<AttemptDto> Execute(GetFilteredAttemptsQuery q)
        {
            var list = _repo
                .GetFilteredAttemptsAsync(q.From, q.To, q.PointId, q.EmployeeId, q.Take)
                .GetAwaiter()
                .GetResult();
            return list;
        }
    }
}
