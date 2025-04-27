using DashboardDomain.Queries;
using DashboardDomain.Queries.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;
using System.Collections.Generic;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IQueryService<GetRecentAttemptsQuery, IEnumerable<AttemptDto>> _getAttempts;

        public DashboardController(
            IQueryService<GetRecentAttemptsQuery, IEnumerable<AttemptDto>> getAttempts)
        {
            _getAttempts = getAttempts;
        }

        [HttpGet("attempts")]
        public ActionResult<IEnumerable<AttemptDto>> GetAttempts([FromQuery] int take = 10)
        {
            var query = new GetRecentAttemptsQuery(take);
            var result = _getAttempts.Execute(query);
            return Ok(result);
        }
    }
}
