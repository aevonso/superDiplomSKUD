using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Model.serverSKUD.Model;

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
        public async Task<ActionResult<AccessMatrix>> Create([FromBody] AccessMatrixCreateDto dto)
        {
            var post = await _db.Posts.FindAsync(dto.PostId);
            if (post == null) return BadRequest(new { message = "PostId не найден" });

            var room = await _db.Rooms.FindAsync(dto.RoomId);
            if (room == null) return BadRequest(new { message = "RoomId не найден" });

            // Удаляем существующую запись для этой пары Post+Room (обеспечиваем уникальность)
            var existing = await _db.AccessMatrices
                .FirstOrDefaultAsync(x => x.PostId == dto.PostId && x.RoomId == dto.RoomId);
            if (existing != null)
            {
                _db.AccessMatrices.Remove(existing);
            }

            var entry = new AccessMatrix
            {
                PostId = dto.PostId,
                RoomId = dto.RoomId,
                IsAccess = dto.IsAccess
            };

            _db.AccessMatrices.Add(entry);
            await _db.SaveChangesAsync();

            await _db.Entry(entry).Reference(x => x.Post).LoadAsync();
            await _db.Entry(entry).Reference(x => x.Room).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
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
        [HttpPut("{id:int}/toggle")]
        public async Task<IActionResult> ToggleAccess(int id)
        {
            var entry = await _db.AccessMatrices.FindAsync(id);
            if (entry == null) return NotFound();

            entry.IsAccess = !entry.IsAccess;
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
