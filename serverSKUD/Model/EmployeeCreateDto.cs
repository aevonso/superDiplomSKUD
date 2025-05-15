// serverSKUD/Model/EmployeeCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace serverSKUD.Model
{
    public class EmployeeCreateDto
    {
        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [MaxLength(150)]
        public string? Patronymic { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MaxLength(30)]
        public string PhoneNumber { get; set; } = null!;

        [Required, MinLength(6)]
        public string Login { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public int DivisionId { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required, MaxLength(4)]
        public string PassportSeria { get; set; } = null!;

        [Required, MaxLength(6)]
        public string PassportNumber { get; set; } = null!;
    }
}
