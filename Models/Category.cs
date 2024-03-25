using System.ComponentModel.DataAnnotations.Schema;

namespace TabGıda.Models
{
    public class Category:BaseModel
    {
        public Guid RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        public virtual ICollection<Food>? Foods { get; set; }
    }
}
