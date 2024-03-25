using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Controllers
{
    [Authorize(Policy="ManagerAccess")]
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<User> _signInManager;
        public FoodController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Foods
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
                        .Include(f => f.Category)
                        .Where(d=>d.isDeleted==false).Where(f => f.Category.RestaurantId == userRestaurant.Id)
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

       

        // GET: Foods/Create
        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

                var userRestaurant = await _context.RestaurantUsers.Where(ru => ru.UserId == user.Id)
                    .Select(ru => ru.Restaurant)
                    .FirstOrDefaultAsync();



                if (userRestaurant != null)
                {
                    var categories = _context.Categories.Where(r=>r.RestaurantId==userRestaurant.Id);
                    ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");


                }


            }
           

            return View();
        }

        // POST: Foods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Price,Description,CategoryId,Id,Name,ImageUrl")] Food food, IFormFile picture)
        {
            FileStream fileStream;
            if (ModelState.IsValid)
            {
                food.Id = Guid.NewGuid();
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string foodPath = Path.Combine(wwwRootPath, @"images");
                string fileName = food.Id.ToString() + ".jpg";
                fileStream = new FileStream(Path.Combine(foodPath, fileName), FileMode.CreateNew);
                picture.CopyTo(fileStream);

                fileStream.Close();
                food.ImageUrl = @"images\" + fileName;

                _context.Add(food);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // GET: Foods/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

            var userRestaurant = await _context.RestaurantUsers.Where(ru => ru.UserId == user.Id)
                .Select(ru => ru.Restaurant)
                .FirstOrDefaultAsync();



            if (userRestaurant != null)
            {
                var categories = _context.Categories.Where(r => r.RestaurantId == userRestaurant.Id);
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");


            }


            return View(food);
        }

        // POST: Foods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Price,Description,CategoryId,Id,Name,isActive,ImageUrl")] Food food, IFormFile picture)
        {
            if (id != food.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (picture != null && picture.Length > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string foodPath = Path.Combine(wwwRootPath, @"images");
                        string fileName = food.Id.ToString() + ".jpg";

                        // Eski resmi sil
                        if (!string.IsNullOrEmpty(food.ImageUrl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, food.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Yeni resmi yükle
                        using (var fileStream = new FileStream(Path.Combine(foodPath, fileName), FileMode.Create))
                        {
                            await picture.CopyToAsync(fileStream);
                            food.ImageUrl = @"images\" + fileName;
                        }
                    }

                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodExists(food.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // GET: Foods/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Foods == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            Food food = _context.Foods!.Find(id);
            food.isDeleted = true;
            _context.Foods.Update(food);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool FoodExists(Guid id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
