using Data.Tables.Data.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    /// <summary>
    /// Запись о попытке прохода сотрудника через точку
    /// </summary>
    public class AccessAttempt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [ForeignKey("PointOfPassage")]
        public int? PointOfPassageId { get; set; }
        public PointOfPassage PointOfPassage { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public bool Success { get; set; }

        [MaxLength(200)]
        public string? FailureReason { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; } = null!;
    }
}
