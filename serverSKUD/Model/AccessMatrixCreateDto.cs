using System.ComponentModel.DataAnnotations;

namespace serverSKUD.Model
{
    namespace serverSKUD.Model
    {
        public class AccessMatrixCreateDto
        {
            [Required]
            public int PostId { get; set; }

            [Required]
            public int RoomId { get; set; }

            public bool IsAccess { get; set; }
        }

        public class AccessMatrixDto
        {
            public int Id { get; set; }
            public bool IsAccess { get; set; }
            public PostDto Post { get; set; }
            public RoomDto Room { get; set; }
        }

        public class PostDto
        {
            public int Id { get; set; }
            public string Name { get; set; } // Изменили Title на Name
            public DivisionDto Division { get; set; }
        }

        public class DivisionDto
        {
            public int Id { get; set; }
            public string Name { get; set; } // Изменили Title на Name
        }

        public class RoomDto
        {
            public int Id { get; set; }
            public string Name { get; set; } // Изменили Title на Name
            public FloorDto Floor { get; set; }
        }

        public class FloorDto
        {
            public int Id { get; set; }
            public string Name { get; set; } // Изменили Title на Name
        }
    }
}
