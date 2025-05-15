using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ControllerBase
    {
        private readonly Connection _db;
        public DivisionController(Connection db) => _db = db;

        // GET: api/Division
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Division>>> GetAll()
        {
            var list = await _db.Divisions.ToListAsync();
            return Ok(list);
        }
    }
}
