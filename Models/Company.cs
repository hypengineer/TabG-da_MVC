using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TabGıda.Models
{
    public class Company:BaseModel
    {
        [StringLength(5, MinimumLength = 5)]
        [Column(TypeName = "char(5)")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [StringLength(200, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { get; set; } = "";

        [Phone]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";

        [EmailAddress]
        [Column(TypeName = "varchar(100)")]
        public string EmailAddress { get; set; } = "";

        [StringLength(11, MinimumLength = 11)]
        [Column(TypeName = "varchar(11)")]
        public string TaxNumber { get; set; } = "";

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? WebAdress { get; set; }

        

        public virtual ICollection<Restaurant>? Restaurants { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
