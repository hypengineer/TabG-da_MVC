using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TabGıda.Data;
using TabGıda.Models;
using Microsoft.AspNetCore.Authorization;
using NuGet.Versioning;

namespace TabGıda.Controllers
{


    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext context, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: User
        [Authorize(Policy = "CompanyAccess")]

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Where(d => d.isDeleted == false).Include(c=>c.Company).ToListAsync();


            return View(users);
        }

        
      

        // GET: User/Create
        [Authorize(Policy = "CompanyAccess")]
        public IActionResult Create()
        {
            var userRole="";
            var user = _signInManager.UserManager.GetUserAsync(User).Result;
            if (user != null)
            {
                 userRole = _signInManager.UserManager.GetRolesAsync(user).Result.FirstOrDefault();
            }

            IEnumerable<IdentityRole> roles;
            if (userRole == "Administrator")
            {
                roles = _roleManager.Roles.Where(r => r.Name == "Admin" || r.Name == "Company Admin");
            }
            else if (userRole == "Admin")
            {
                roles = _roleManager.Roles.Where(r => r.Name == "Company Admin" );
            }
            else 
            {
                roles = _roleManager.Roles.Where(r =>  r.Name == "Manager");
            }
            
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Name");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "CompanyAccess")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserName,Email,PhoneNumber,CompanyId,RoleId")] User user, string password, string selectedRole, Guid RestaurantId)
        {
            
            if (ModelState.IsValid)
            {
                await _signInManager.UserManager.CreateAsync(user, password);
                var role = await _roleManager.FindByIdAsync(selectedRole);
                
                //Composit key ile birbirine bağlı tabloya veri ekleyip idleri eşleştirmek
                if (role != null)
                {
                    if (role.Name== "Manager")
                    {
                        var restaurantUser = new RestaurantUser { RestaurantId = RestaurantId, UserId = user.Id };
                        _context.RestaurantUsers.Add(restaurantUser);
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.UserManager.AddToRoleAsync(user,role.Name);
                    
                 

                    return RedirectToAction("Index");
                }

            }
            await _context.SaveChangesAsync();
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");

            return View(user);
        }

        // GET: User/Edit/5
        [Authorize(Policy = "CompanyAccess")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _signInManager.UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
            ViewData["RoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");

            
            var currentRole = await _signInManager.UserManager.GetRolesAsync(user);
            ViewBag.CurrentRole = currentRole.FirstOrDefault();

            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CompanyAccess")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,isActive,UserName,Email,PhoneNumber,CompanyId,RoleId")] User user,string selectedRole)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _signInManager.UserManager.FindByIdAsync(id);
                    currentUser.Name = user.Name;
                    currentUser.isDeleted = user.isDeleted;
                    currentUser.isActive = user.isActive;
                    currentUser.UserName = user.UserName;
                    currentUser.Email = user.Email;
                    currentUser.PhoneNumber = user.PhoneNumber;
                    currentUser.CompanyId = user.CompanyId;

                    // Kullanıcının rolünü değiştirme işlemi
                    var currentRole = await _signInManager.UserManager.GetRolesAsync(currentUser);
                    var newRole = await _roleManager.FindByIdAsync(selectedRole);

                    

                    if (newRole != null && !currentRole.Contains(newRole.Name))
                    {
                        await _signInManager.UserManager.RemoveFromRolesAsync(currentUser, currentRole);
                        await _signInManager.UserManager.AddToRoleAsync(currentUser, newRole.Name);
                    }



                    await _signInManager.UserManager.UpdateAsync(currentUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
            ViewData["RoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");
            return View(user);
        }

        // GET: User/Delete/5
        [Authorize(Policy = "CompanyAccess")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _signInManager.UserManager.Users == null)
            {
                return NotFound();
            }

            var user = await _signInManager.UserManager.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [Authorize(Policy = "CompanyAccess")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_signInManager.UserManager.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _signInManager.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                
                user.isDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public IActionResult Login(string UserName, string Password)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            User user = _signInManager.UserManager.FindByNameAsync(UserName).Result;

            // Kullanıcı null değilse işlemi devam ettir
            if (user != null)
            {
                // Kullanıcıyı giriş yapmaya çalış
                 signInResult = _signInManager.PasswordSignInAsync(user, Password, false, false).Result;

                // Giriş başarılı ise
                if (signInResult.Succeeded)
                {
                    // Ana sayfaya yönlendir
                    return RedirectToAction("Index", "Home");
                }
            }

            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Kullanıcıyı oturumdan çıkar
            await _signInManager.SignOutAsync();

           
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
