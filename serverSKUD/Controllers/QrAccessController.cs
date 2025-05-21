// QrAccessController.cs
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class QrAccessController : ControllerBase
{
    private readonly Connection _db;
    public QrAccessController(Connection db) => _db = db;

    [HttpPost("check")]
    public async Task<IActionResult> CheckAccess([FromBody] QrAccessDto dto)
    {
        var access = await _db.AccessMatrices
            .AnyAsync(x => x.PostId == dto.PostId && x.RoomId == dto.RoomId && x.IsAccess);

        return Ok(new { hasAccess = access });
    }

    [HttpGet("/api/Post/withAccess")]
    public async Task<IActionResult> GetPostsWithAccess([FromQuery] int roomId)
    {
        var posts = await _db.Posts
            .Include(p => p.Division)
            .Select(p => new
            {
                p.Id,
                Name = p.Name,
                Division = p.Division.Name,
                HasAccess = _db.AccessMatrices
                    .Any(m => m.PostId == p.Id && m.RoomId == roomId && m.IsAccess)
            })
            .ToListAsync();

        return Ok(posts);
    }
}



public class QrAccessDto
    {
        public int PostId { get; set; }
        public int RoomId { get; set; }
    }


