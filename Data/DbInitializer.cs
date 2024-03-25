using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Data
{
    public class DbInitializer
    {
        public DbInitializer(ApplicationDbContext? context, RoleManager<IdentityRole>? roleManager, UserManager<User> userManager)
        {
            
            IdentityRole identityRole;
            User User;
            Company? company = null;
            if (context != null)
            {
                context.Database.Migrate();
               
                
                if (roleManager != null)
                {
                    if (roleManager.Roles.Count() == 0)
                    {
                        identityRole = new IdentityRole("Administrator");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("Admin");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("Company Admin");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("Manager");
                        roleManager.CreateAsync(identityRole).Wait();
                    }
                }

                if (context.Companies.Count() == 0)
                {
                    company = new Company();
                    company.Address = "adres";
                    company.EmailAddress = "abc@def.com";
                    company.Name = "Easy";
                    company.Phone = "1112223344";
                    company.PostalCode = "12345";
                    company.DateCreated = DateTime.Now;
                    company.TaxNumber = "11111111111";
                    context.Companies.Add(company);
                }
                if (userManager != null)
                {
                    if (userManager.Users.Count() == 0)
                    {
                        if (company != null)
                        {
                            User user = new User();
                            user.UserName = "Administrator";
                            user.Name = "Administrator";
                            user.Email = "abc@def.com";
                            user.PhoneNumber = "1112223344";
                            user.Company = company;
                            userManager.CreateAsync(user, "Admin123!").Wait();
                            userManager.AddToRoleAsync(user, "Administrator").Wait();
                        }
                    }
                }
            }
        }
    }
}
