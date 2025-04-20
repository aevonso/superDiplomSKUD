using AutorizationDomain.Queries.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain
{
    public interface IAuthRepository
    {
        Employeer? Autorization(string login, string password);
        void UpdateRefreshToken(string login, string refreshToken, DateTime expiry);

        DateTime? GetRefreshExpiry(string refreshToken);

        Employeer? GetByRefreshToken(string refreshToken);


    }
}
