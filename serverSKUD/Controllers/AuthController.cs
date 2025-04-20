using AutorizationDomain.Queries.Object;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IQueryService<EntryDto, AuthResult> _loginService;
        private readonly IQueryService<RefreshDto, AuthResult> _refreshService;

        public AuthController(
            IQueryService<EntryDto, AuthResult> loginService,
            IQueryService<RefreshDto, AuthResult> refreshService)
        {
            _loginService = loginService;
            _refreshService = refreshService;
        }

        [HttpPost("login")]
        public ActionResult<AuthResult> Login([FromBody] EntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = _loginService.Execute(dto);
            if (authResult == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            return Ok(authResult);
        }

        [HttpPost("refresh")]
        public ActionResult<AuthResult> Refresh([FromBody] RefreshDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Не указан refreshToken" });

            var authResult = _refreshService.Execute(dto);
            if (authResult == null)
                return Unauthorized(new { message = "Неверный или просроченный refreshToken" });

            return Ok(authResult);
        }
    }
}
