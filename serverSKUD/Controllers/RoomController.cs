using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly Connection _db;
        public RoomController(Connection db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAll([FromQuery] int? floorId)
        {
            var q = _db.Rooms.Include(r => r.Floor).AsQueryable();
            if (floorId.HasValue)
                q = q.Where(r => r.FloorId == floorId.Value);
            var list = await q.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Room>> GetById(int id)
        {
            var r = await _db.Rooms
                .Include(rm => rm.Floor)
                .FirstOrDefaultAsync(rm => rm.Id == id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Create([FromBody] Room dto)
        {
            _db.Rooms.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Room dto)
        {
            var r = await _db.Rooms.FindAsync(id);
            if (r == null) return NotFound();
            r.Name = dto.Name;
            r.FloorId = dto.FloorId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Rooms.FindAsync(id);
            if (r == null) return NotFound();
            _db.Rooms.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
