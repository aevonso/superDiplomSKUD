using AuntificationDomain;
using AuntificationDomain.Queries.Object;
using serviceSKUD;
using System.Threading.Tasks;

namespace AuthenticationDomain.Queries
{
    public class Generate2FaQueryService
        : IQueryService<TwoFactorGenerateDto, Task<string>>
    {
        private readonly I2FaRepository _repo;

        public Generate2FaQueryService(I2FaRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<string> Execute(TwoFactorGenerateDto dto)
            => _repo.GenerateCodeAsync(dto.Login);
    }
}
