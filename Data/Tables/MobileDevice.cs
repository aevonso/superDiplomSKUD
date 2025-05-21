using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tables.Data.Tables;

namespace Data.Tables
{
    public class MobileDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployerId { get; set; }
        public Employee Employee { get; set; } = null!;
        public string DeviceName { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; }
    }
}