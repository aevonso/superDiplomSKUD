using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessMatrixController : ControllerBase
    {
        private readonly Connection _db;
        public AccessMatrixController(Connection db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessMatrix>>> GetAll(
            [FromQuery] int? floorId,
            [FromQuery] int? divisionId)
        {
            var q = _db.AccessMatrices
                .Include(x => x.Post).ThenInclude(p => p.Division)
                .Include(x => x.Room).ThenInclude(r => r.Floor)
                .AsQueryable();

            if (floorId.HasValue)
                q = q.Where(x => x.Room.FloorId == floorId.Value);

            if (divisionId.HasValue)
                q = q.Where(x => x.Post.DivisionId == divisionId.Value);

            var list = await q.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccessMatrix>> GetById(int id)
        {
            var x = await _db.AccessMatrices
                .Include(a => a.Post)
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (x == null) return NotFound();
            return Ok(x);
        }

        [HttpPost]
        public async Task<ActionResult<AccessMatrix>> Create([FromBody] AccessMatrix dto)
        {
            _db.AccessMatrices.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccessMatrix dto)
        {
            var x = await _db.AccessMatrices.FindAsync(id);
            if (x == null) return NotFound();
            x.PostId = dto.PostId;
            x.RoomId = dto.RoomId;
            x.IsAccess = dto.IsAccess;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var x = await _db.AccessMatrices.FindAsync(id);
            if (x == null) return NotFound();
            _db.AccessMatrices.Remove(x);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
