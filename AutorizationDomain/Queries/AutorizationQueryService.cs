﻿// AutorizationDomain/Queries/AutorizationQueryService.cs
using System;
using AutorizationDomain.Queries.Object;
using serviceSKUD;

namespace AutorizationDomain.Queries
{
    public class AutorizationQueryService
        : IQueryService<EntryDto, AuthResult>
    {
        private readonly IAuthRepository _repo;
        private readonly ITokenService _tokenSvc;

        public AutorizationQueryService(
            IAuthRepository repo,
            ITokenService tokenSvc)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _tokenSvc = tokenSvc ?? throw new ArgumentNullException(nameof(tokenSvc));
        }

        public AuthResult Execute(EntryDto dto)
        {
            var user = _repo.Autorization(dto.Login, dto.Password);
            if (user == null) return null!;

            return _tokenSvc.CreateTokens(user);
        }
    }
}
