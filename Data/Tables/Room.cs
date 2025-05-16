using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Name { get; set; } = null!;  // "355", "366" и т.п.

        [ForeignKey("Floor")]
        public int FloorId { get; set; }
        public Floor Floor { get; set; } = null!;

        // Навигация: комната —> несколько записей в матрице
        public ICollection<AccessMatrix> AccessEntries { get; set; } = new List<AccessMatrix>();
    }
}
