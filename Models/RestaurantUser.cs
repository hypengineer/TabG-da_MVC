using System.ComponentModel.DataAnnotations.Schema;

namespace TabGıda.Models
{
    public class RestaurantUser
    {
       

        public Guid RestaurantId { get; set; } 
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; } = "";
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
