using serviceSKUD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationDomain.Queries.Object
{
    public class EntryDto : IQuery
    {
        [MinLength(3, ErrorMessage = "Минимум логин 3 буквы")]
        [MaxLength(20, ErrorMessage = "Пароль логин 20 букв")]
        public string Login { get; set; } = null!;

        [MinLength(3, ErrorMessage = "Минимум пароль 3 буквы")]
        [MaxLength(20, ErrorMessage = "Пароль максимум 20 букв")]
        public string Password { get; set; } = null!;
    }
}
