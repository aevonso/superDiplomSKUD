using AuntificationDomain;
using AuntificationDomain.Queries.Object;
using serviceSKUD;
using System.Threading.Tasks;

namespace AuthenticationDomain.Queries
{
    public class Validate2FaQueryService
        : IQueryService<TwoFactorValidateDto, Task<TwoFactorResult>>
    {
        private readonly I2FaRepository _repo;

        public Validate2FaQueryService(I2FaRepository repo)
        {
            _repo = repo;
        }

        public Task<TwoFactorResult> Execute(TwoFactorValidateDto dto)
            => ValidateAsync(dto);

        private async Task<TwoFactorResult> ValidateAsync(TwoFactorValidateDto dto)
        {
            var ok = await _repo.ValidateCodeAsync(dto.Login, dto.Code);
            return new TwoFactorResult
            {
                Success = ok,
                Message = ok ? null : "Неверный или просроченный код"
            };
        }
    }
}
