using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TabGýda.Data;
using TabGýda.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministratorAccess", policy => policy.RequireRole("Administrator"));

    options.AddPolicy("AdminAccess", policy =>
        policy.RequireAssertion(context =>
                    context.User.IsInRole("Adminstrator")
                    || context.User.IsInRole("Admin")));

    options.AddPolicy("CompanyAccess", policy =>
        policy.RequireAssertion(context =>
                    context.User.IsInRole("Administrator")
                    || context.User.IsInRole("Admin")
                    || context.User.IsInRole("Company Admin")));


        options.AddPolicy("ManagerAccess", policy =>
        policy.RequireAssertion(context =>
                    context.User.IsInRole("Company Admin")
                    || context.User.IsInRole("Manager")));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

ApplicationDbContext? context = app.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();
RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
UserManager<User>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<User>>();
DbInitializer dbInitializer = new DbInitializer(context, roleManager, userManager);

app.Run();
