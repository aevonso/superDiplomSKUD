using serviceSKUD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace AutorizationDomain.Queries.Object
{
    [Serializable]
    public class EntryDto : IQuery
    {
        [MinLength(3, ErrorMessage = "Минимум логин 3 буквы")]
        [MaxLength(20, ErrorMessage = "Пароль логин 20 букв")]
        [JsonPropertyName("login")]  // Применение camelCase
        public string Login { get; set; } = null!;

        [MinLength(3, ErrorMessage = "Минимум пароль 3 буквы")]
        [MaxLength(20, ErrorMessage = "Пароль максимум 20 букв")]
        [JsonPropertyName("password")]  // Применение camelCase
        public string Password { get; set; } = null!;
    }
}
