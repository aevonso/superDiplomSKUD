using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorController : ControllerBase
    {
        private readonly Connection _db;
        public FloorController(Connection db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Floor>>> GetAll()
        {
            var list = await _db.Floors.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Floor>> GetById(int id)
        {
            var f = await _db.Floors.FindAsync(id);
            if (f == null) return NotFound();
            return Ok(f);
        }

        [HttpPost]
        public async Task<ActionResult<Floor>> Create([FromBody] Floor dto)
        {
            _db.Floors.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Floor dto)
        {
            var f = await _db.Floors.FindAsync(id);
            if (f == null) return NotFound();
            f.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var f = await _db.Floors.FindAsync(id);
            if (f == null) return NotFound();
            _db.Floors.Remove(f);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
