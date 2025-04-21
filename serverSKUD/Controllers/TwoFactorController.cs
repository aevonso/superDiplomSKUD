using AuntificationDomain;
using AuntificationDomain.Queries.Object;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;
using System.Threading.Tasks;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("auth/2fa")]
    public class TwoFactorController : ControllerBase
    {
        private readonly IQueryService<TwoFactorGenerateDto, Task<string>> _genSvc;
        private readonly IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>> _valSvc;
        private readonly IEmailSender _email;
        private readonly I2FaRepository _twoFaRepo;

        public TwoFactorController(
            IQueryService<TwoFactorGenerateDto, Task<string>> genSvc,
            IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>> valSvc,
            IEmailSender emailSender,
            I2FaRepository twoFaRepo)
        {
            _genSvc = genSvc;
            _valSvc = valSvc;
            _email = emailSender;
            _twoFaRepo = twoFaRepo;
        }


        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] TwoFactorGenerateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var code = await _genSvc.Execute(dto);

            var email = await _twoFaRepo.GetEmailByLoginAsync(dto.Login);
            if (string.IsNullOrEmpty(email))
                return NotFound(new { message = "Email пользователя не найден" });

            var subject = "Ваш код 2FA для СКУД НАТК";
            var body = $"<p>Ваш код для входа: <b>{code}</b></p>";
            await _email.SendCodeAsync(email, subject, body);

            return Ok(new { message = "Код отправлен на почту" });
        }

        [HttpPost("validate")]
        public async Task<ActionResult<TwoFactorResult>> Validate([FromBody] TwoFactorValidateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _valSvc.Execute(dto);
            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
