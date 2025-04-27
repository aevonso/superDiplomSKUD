using DashboardDomain.IRepository;
using DashboardDomain.Queries.Object;
using serviceSKUD;
using System.Collections.Generic;
using System.Linq;

namespace DashboardDomain.Queries
{
    public class GetRecentAttemptsQueryService
        : IQueryService<GetRecentAttemptsQuery, IEnumerable<AttemptDto>>
    {
        private readonly IAccessAttemptRepository _repo;
        public GetRecentAttemptsQueryService(IAccessAttemptRepository repo) => _repo = repo;

        public IEnumerable<AttemptDto> Execute(GetRecentAttemptsQuery q)
        {
            var list = _repo.GetRecentAttemptsAsync(q.Take)
                           .GetAwaiter()
                           .GetResult();
            return list;
        }
    }
}
