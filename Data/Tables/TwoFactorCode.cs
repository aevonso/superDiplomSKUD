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
   
    public class TwoFactorCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Внешний ключ на сотрудника
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [Required, MaxLength(6)]
        public string Code { get; set; } = null!;

        //срок кода
        public DateTime Expiry { get; set; }

        // Флаг «уже использован»
        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
