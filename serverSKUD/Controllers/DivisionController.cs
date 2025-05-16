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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Division>>> GetAll()
        {
            var list = await _db.Divisions.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Division>> GetById(int id)
        {
            var d = await _db.Divisions.FindAsync(id);
            if (d == null) return NotFound();
            return Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<Division>> Create([FromBody] Division dto)
        {
            _db.Divisions.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Division dto)
        {
            var d = await _db.Divisions.FindAsync(id);
            if (d == null) return NotFound();
            d.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Divisions.FindAsync(id);
            if (d == null) return NotFound();
            _db.Divisions.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
