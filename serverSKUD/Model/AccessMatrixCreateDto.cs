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
    }
}
