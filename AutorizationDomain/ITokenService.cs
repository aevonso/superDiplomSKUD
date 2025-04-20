using AutorizationDomain.Queries.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain
{
    public interface ITokenService
    {
        AuthResult CreateTokens(Employeer user);
    }
}
