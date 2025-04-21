using serviceSKUD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuntificationDomain.Queries.Object
{
    public class TwoFactorGenerateDto : IQuery
    {
        [MinLength(3)] 
        public string Login { get; set; } = null!;
    }
}
