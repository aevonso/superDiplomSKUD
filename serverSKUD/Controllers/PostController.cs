using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly Connection _db;
        public PostController(Connection db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            var list = await _db.Posts
                .Include(p => p.Division)
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var p = await _db.Posts
                .Include(x => x.Division)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Create([FromBody] Post dto)
        {
            _db.Posts.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Post dto)
        {
            var p = await _db.Posts.FindAsync(id);
            if (p == null) return NotFound();
            p.Name = dto.Name;
            p.DivisionId = dto.DivisionId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Posts.FindAsync(id);
            if (p == null) return NotFound();
            _db.Posts.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
