// AutorizationDomain/Queries/RefreshQueryService.cs
using System;
using AutorizationDomain.Queries.Object;
using serviceSKUD;

namespace AutorizationDomain.Queries
{
    public class RefreshQueryService
        : IQueryService<RefreshDto, AuthResult>
    {
        private readonly IAuthRepository _repo;
        private readonly ITokenService _tokenSvc;

        public RefreshQueryService(
            IAuthRepository repo,
            ITokenService tokenSvc)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _tokenSvc = tokenSvc ?? throw new ArgumentNullException(nameof(tokenSvc));
        }

        public AuthResult Execute(RefreshDto dto)
        {
            var user = _repo.GetByRefreshToken(dto.RefreshToken);
            if (user == null) return null!;

            var expiry = _repo.GetRefreshExpiry(dto.RefreshToken);
            if (expiry == null || expiry.Value < DateTime.UtcNow)
                return null!;

            return _tokenSvc.CreateTokens(user);
        }
    }
}
