using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Controllers
{
    [Authorize(Policy ="ManagerAccess")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public CategoryController(ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            List<Category> nullcategory = new List<Category>();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

                var userRestaurant = await _context.RestaurantUsers.Where(ru => ru.UserId == user.Id)
                    .Select(ru => ru.Restaurant)
                    .FirstOrDefaultAsync();



                if (userRestaurant != null)
                {
                    var categories = _context.Categories.Where(d=>d.isDeleted==false)
                        .Where(f => f.RestaurantId == userRestaurant.Id)
                        .ToList();



                    if (categories.Any())
                    {
                        return View(categories);
                    }
                    else
                    {
                        // Food modeli null olduğunda null değeri ile view'e gönder
                        return View(nullcategory);

                    }
                }

                else
                {
                    return View(nullcategory);
                }



            }
            return View(nullcategory);
        }

       

        // GET: Category/Create
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
                    var restaurants = _context.Restaurants.Where(r => r.Id == userRestaurant.Id);
                    ViewData["RestaurantId"] = new SelectList(restaurants, "Id", "Name");


                }


            }
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RestaurantId,Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Name", category.RestaurantId);
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

            var userRestaurant = await _context.RestaurantUsers.Where(ru => ru.UserId == user.Id)
                .Select(ru => ru.Restaurant)
                .FirstOrDefaultAsync();



            if (userRestaurant != null)
            {
                var restaurants = _context.Restaurants.Where(r => r.Id == userRestaurant.Id);
                ViewData["RestaurantId"] = new SelectList(restaurants, "Id", "Name");


            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RestaurantId,Id,Name,isActive")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "Name", category.RestaurantId);
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {

            Category? category = _context.Categories!.Where(c => c.Id == id).Include(f => f.Foods).FirstOrDefault();
            if (category != null)
            {
                category.isDeleted = true;



                foreach (Food f in category.Foods)
                {
                    f.isDeleted = true;

                }

            }
            _context.Categories.Update(category);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}