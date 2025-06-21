using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serverSKUD.Model;
using serverSKUD.Model.serverSKUD.Model;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessMatrixController : ControllerBase
    {
        private readonly Connection _db;
        public AccessMatrixController(Connection db) => _db = db;

        // Получение всех записей матрицы доступа с возможностью фильтрации
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccessMatrixDto>> GetById(int id)
        {
            var accessMatrix = await _db.AccessMatrices
                .Include(a => a.Post).ThenInclude(p => p.Division)
                .Include(a => a.Room).ThenInclude(r => r.Floor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (accessMatrix == null) return NotFound();

            return Ok(MapToDto(accessMatrix));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessMatrixDto>>> GetAll(
            [FromQuery] int? floorId,
            [FromQuery] int? divisionId)
        {
            var query = _db.AccessMatrices
                .Include(x => x.Post).ThenInclude(p => p.Division)
                .Include(x => x.Room).ThenInclude(r => r.Floor)
                .AsQueryable();

            if (floorId.HasValue)
                query = query.Where(x => x.Room.FloorId == floorId.Value);

            if (divisionId.HasValue)
                query = query.Where(x => x.Post.DivisionId == divisionId.Value);

            var accessMatrices = await query.ToListAsync();

            return Ok(accessMatrices.Select(MapToDto));
        }

        // Метод для преобразования Entity в DTO
        private AccessMatrixDto MapToDto(AccessMatrix accessMatrix)
        {
            return new AccessMatrixDto
            {
                Id = accessMatrix.Id,
                IsAccess = accessMatrix.IsAccess,
                Post = accessMatrix.Post == null ? null : new PostDto
                {
                    Id = accessMatrix.Post.Id,
                    Name = accessMatrix.Post.Name,
                    Division = accessMatrix.Post.Division == null ? null : new DivisionDto
                    {
                        Id = accessMatrix.Post.Division.Id,
                        Name = accessMatrix.Post.Division.Name
                    }
                },
                Room = accessMatrix.Room == null ? null : new RoomDto
                {
                    Id = accessMatrix.Room.Id,
                    Name = accessMatrix.Room.Name,
                    Floor = accessMatrix.Room.Floor == null ? null : new FloorDto
                    {
                        Id = accessMatrix.Room.Floor.Id,
                        Name = accessMatrix.Room.Floor.Name
                    }
                }
            };
        }

        // Создание новой записи в матрице доступа
        [HttpPost]
        public async Task<ActionResult<AccessMatrix>> Create([FromBody] AccessMatrixCreateDto dto)
        {
            var post = await _db.Posts.FindAsync(dto.PostId);
            if (post == null) return BadRequest(new { message = "PostId не найден" });

            var room = await _db.Rooms.FindAsync(dto.RoomId);
            if (room == null) return BadRequest(new { message = "RoomId не найден" });

            // Проверяем, существует ли уже запись для этой пары PostId и RoomId
            var existing = await _db.AccessMatrices
                .FirstOrDefaultAsync(x => x.PostId == dto.PostId && x.RoomId == dto.RoomId);

            if (existing != null)
            {
                return BadRequest(new { message = "Запись для данной пары PostId и RoomId уже существует" });
            }

            var entry = new AccessMatrix
            {
                PostId = dto.PostId,
                RoomId = dto.RoomId,
                IsAccess = dto.IsAccess
            };

            _db.AccessMatrices.Add(entry);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }


        // Обновление записи матрицы доступа
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
        
        // Включение/выключение доступа
        [HttpPut("{id:int}/toggle")]
        public async Task<IActionResult> ToggleAccess(int id)
        {
            var entry = await _db.AccessMatrices.FindAsync(id);
            if (entry == null) return NotFound();

            entry.IsAccess = !entry.IsAccess;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Удаление записи матрицы доступа
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
