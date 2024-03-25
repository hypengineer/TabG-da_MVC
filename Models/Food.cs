using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TabGıda.Models
{
    public class Food:BaseModel
    {
        [Range(0, float.MaxValue)]
        public double Price { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? Description { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; } = "";

        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
