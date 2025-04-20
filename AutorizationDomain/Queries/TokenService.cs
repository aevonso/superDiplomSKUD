using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutorizationDomain.Queries.Object;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using serviceSKUD;

namespace AutorizationDomain
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;
        private readonly IAuthRepository _repo;

        public TokenService(
            IOptions<JwtSettings> jwtOptions,
            IAuthRepository repo)
        {
            _jwt = jwtOptions.Value;
            _repo = repo;
        }

        public AuthResult CreateTokens(Employeer user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login)
            }.Concat(
                user.Roles.Select(r => new Claim(ClaimTypes.Role, r))
            );

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes);

            var jwtToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = Guid.NewGuid().ToString();
            var refreshExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays);
            _repo.UpdateRefreshToken(user.Login, refreshToken, refreshExpiry);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expires
            };
        }
    }
}
