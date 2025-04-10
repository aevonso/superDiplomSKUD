using AutorizationDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries
{
    public class AutorizationQueryService : IQueryService<EntryDto, Employeer?>
    {
        private readonly IAuthRepository _authRepository;

        public AutorizationQueryService(IAuthRepository authRepository)
        {
            ArgumentNullException.ThrowIfNull(authRepository, nameof(authRepository));
            _authRepository = authRepository;
        }
        public Employeer? Execute(EntryDto obj)
        {
            if (obj == null)
                return null;

            return _authRepository.Autorization(obj.Login, obj.Password);
        }
    }
}
