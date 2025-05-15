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

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            var list = await _db.Posts.ToListAsync();
            return Ok(list);
        }
    }
}
