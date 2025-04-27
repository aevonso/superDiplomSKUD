using AutorizationDomain.Queries.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using serviceSKUD;

namespace serverSKUD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class QrController : ControllerBase
    {
        private readonly ICommandService<GenerateQrDto> _generateQrService;

        public QrController(ICommandService<GenerateQrDto> generateQrService)
        {
            _generateQrService = generateQrService;
        }

        [HttpPost("generate")]
        public IActionResult Generate([FromBody] GenerateQrDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _generateQrService.Execute(dto);

            return Ok(new { Message = "QR-код сгенерирован и сохранён." });
        }
    }
}
