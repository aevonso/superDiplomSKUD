using System;

namespace AutorizationDomain.Queries.Object
{
    public class AuthResult
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
