using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuntificationDomain
{
    public interface I2FaRepository
    {
        Task<string> GenerateCodeAsync(string login);
        Task<bool> ValidateCodeAsync(string login, string code);
        Task<string?> GetEmailByLoginAsync(string login);

    }
}
