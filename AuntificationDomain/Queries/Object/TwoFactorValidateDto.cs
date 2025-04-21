using serviceSKUD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuntificationDomain.Queries.Object
{
    public class TwoFactorValidateDto : IQuery
    {
        [MinLength(3)] 
        public string Login { get; set; } = null!;
        [MinLength(1)] 
        public string Code { get; set; } = null!;
    }
}
