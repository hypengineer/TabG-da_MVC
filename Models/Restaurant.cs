using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TabGıda.Models
{
    public class Restaurant: BaseModel
    {
        [Required]
        [Phone]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";

        [Required]
        [StringLength(5, MinimumLength = 5)]
        [Column(TypeName = "char(5)")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [Required]
        [StringLength(200, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(200)")]
        public string AddressDetail { get; set; } = "";

        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }

        public virtual ICollection<RestaurantUser>? RestaurantUsers { get; set; }
    }
}
