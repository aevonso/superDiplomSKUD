using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    namespace Data.Tables
    {
        public class Employee
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string LastName { get; set; } = null!;

            [Required]
            [MaxLength(100)]
            public string FirstName { get; set; } = null!;

            [MaxLength(150)]
            public string Patronymic { get; set; } = null!;

            [MaxLength(30)]
            public string PhoneNumber { get; set; } = null!;

            [Required]
            public string Login { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;

            [ForeignKey("Division")]
            public int DivisionId { get; set; }
            public Division Division { get; set; } = null!;

            [ForeignKey("Post")]
            public int PostId { get; set; }
            public Post Post { get; set; } = null!;

            [MaxLength(9)]
            public string PassportNumber { get; set; } = null!;

            public string Email { get; set; } = null!;

            [MaxLength(6)]
            public string PassportSeria { get; set; } = null!;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public string? RefreshToken { get; set; }
            public DateTime? RefreshTokenExpiryTime { get; set; }
        }

    }
}
