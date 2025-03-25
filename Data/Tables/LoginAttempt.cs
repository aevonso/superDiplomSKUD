using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class LoginAttempt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("MobileDevice")]
        public int MobileId { get; set; }
        public MobileDevice MobileDevice { get; set; } = null!;

        public TimeSpan DateLogin { get; set; }

        public bool IsSuccess { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [ForeignKey("PointOfPassage")]
        public int IdPoint { get; set; }
        public PointOfPassage PointOfPassage { get; set; } = null!;
    }
}