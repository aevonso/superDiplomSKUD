using AutorizationDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries
{
    public class CreatePrincipalQueryService
                : IQueryService<Employeer, ClaimsPrincipal>
    {
        public ClaimsPrincipal Execute(Employeer obj)
        {
            List<Claim> claims = new List<Claim>();
            foreach (var name in obj.Roles)
            {
                claims.Add(new Claim("role", name));
            }
            var identity = new ClaimsIdentity(claims,
                                            "RolesClaim",
                                            ClaimTypes.Name,
                                            ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
    }
}
