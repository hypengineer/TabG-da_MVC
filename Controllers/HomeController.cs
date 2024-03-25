using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;



        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Food> nullfoods = new List<Food>();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);
                
                    var userRestaurant = await _context.RestaurantUsers.Where(ru => ru.UserId == user.Id)
                        .Select(ru => ru.Restaurant)
                        .FirstOrDefaultAsync();



                    if (userRestaurant != null)
                    {
                        var foods = _context.Foods
                            .Include(f => f.Category).Where(d=>d.isDeleted==false)
                            .Where(f => f.Category.RestaurantId == userRestaurant.Id)
                            .ToList();



                        if (foods.Any())
                        {
                            return View(foods);
                        }
                        else
                        {
                        // Food modeli null olduğunda null değeri ile view'e gönder
                        return View(nullfoods);

                    }
                }

                    else
                {
                    return View(nullfoods);
                }



            }
            return View(nullfoods);







        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
