using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TabGıda.Models
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(200, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";
        public bool isDeleted { get; set; } = false;
        public bool isActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
