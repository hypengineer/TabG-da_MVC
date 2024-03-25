using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TabGıda.Models;

namespace TabGıda.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Company>? Companies { get; set; }
        public DbSet<Food>? Foods { get; set; }
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Category>? Categories { get; set; }

        public DbSet<RestaurantUser>? RestaurantUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId);

            

            builder.Entity<RestaurantUser>().HasKey(ru => new { ru.RestaurantId, ru.UserId });

            builder.Entity<RestaurantUser>()
                .HasOne(ru => ru.Restaurant)
                .WithMany(ru => ru.RestaurantUsers)
                .HasForeignKey(ru => ru.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RestaurantUser>()
                .HasOne(ru => ru.User)
                .WithMany(ru => ru.RestaurantUsers)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }



    }
}
