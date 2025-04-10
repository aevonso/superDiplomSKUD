using AutorizationDomain.Queries.Object;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IQueryService<EntryDto, Employeer?> _authQuery;

        public AuthController(IQueryService<EntryDto, Employeer?> authQuery)
        {
            _authQuery = authQuery;
        }

        [HttpPost("login")]
        public ActionResult<Employeer> Login([FromBody] EntryDto entryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _authQuery.Execute(entryDto);
            if(result==null)
            {
                return Unauthorized("Неверный логин или пароль");
            }
            return Ok(result);
        }
    }
}
