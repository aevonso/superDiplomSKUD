using DashboardDomain.Queries;
using DashboardDomain.Queries.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;
using System;
using System.Collections.Generic;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IQueryService<GetDashboardStatsQuery, DashboardStatsDto> _statsSvc;
        private readonly IQueryService<GetFilteredAttemptsQuery, IEnumerable<AttemptDto>> _attSvc;

        public DashboardController(
            IQueryService<GetDashboardStatsQuery, DashboardStatsDto> statsSvc,
            IQueryService<GetFilteredAttemptsQuery, IEnumerable<AttemptDto>> attSvc)
        {
            _statsSvc = statsSvc;
            _attSvc = attSvc;
        }


        [HttpGet("stats")]
        public ActionResult<DashboardStatsDto> GetStats()
        {
            var dto = _statsSvc.Execute(new GetDashboardStatsQuery());
            return Ok(dto);
        }


        [HttpGet("attempts")]
        public ActionResult<IEnumerable<AttemptDto>> GetAttempts(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? pointId,
            [FromQuery] int? employeeId,
            [FromQuery] int take = 10)
        {
            var query = new GetFilteredAttemptsQuery(from, to, pointId, employeeId, take);
            var list = _attSvc.Execute(query);
            return Ok(list);
        }
    }
}
