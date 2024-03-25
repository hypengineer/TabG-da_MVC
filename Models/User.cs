using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TabGıda.Models
{
    public class User:IdentityUser
    {
       
        [Required]
        [StringLength(200, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";
        public bool isDeleted { get; set; } = false;
        public bool isActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [StringLength(100, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(100)")]
        public override string UserName { get; set; } = "";

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        [EmailAddress]
        public override string Email { get; set; } = "";

        [Phone]
        [Column("varchar(30)")]
        public override string PhoneNumber { get; set; } = "";


        public Guid CompanyId { set; get; }
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }


        public virtual ICollection<RestaurantUser>? RestaurantUsers { get; set; }

    }
}
