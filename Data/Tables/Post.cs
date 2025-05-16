using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [ForeignKey("Division")]
        public int DivisionId { get; set; }
        public Division Division { get; set; } = null!;

        // Навигация: одна должность может иметь много записей в матрице
        public ICollection<AccessMatrix> AccessEntries { get; set; } = new List<AccessMatrix>();
    }

}
