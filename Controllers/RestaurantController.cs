using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Controllers
{
    [Authorize(Policy = "CompanyAccess")]

    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Restaurant
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Restaurants.Include(r => r.Company).Where(d=>d.isDeleted == false);
            return View(await applicationDbContext.ToListAsync());
        }

      

        // GET: Restaurant/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Phone,PostalCode,AddressDetail,CompanyId,Id,Name")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                restaurant.Id = Guid.NewGuid();
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", restaurant.CompanyId);
            return View(restaurant);
        }

        // GET: Restaurant/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", restaurant.CompanyId);
            return View(restaurant);
        }

        // POST: Restaurant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Phone,PostalCode,AddressDetail,CompanyId,Id,Name,isActive")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", restaurant.CompanyId);
            return View(restaurant);
        }

        // GET: Restaurant/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .Include(r => r.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {

            Restaurant? restaurant = _context.Restaurants!.Where(c => c.Id == id).Include(c => c.Categories).ThenInclude(f => f.Foods).FirstOrDefault();
            if (restaurant != null)
            {
                restaurant.isDeleted = true;
                
                    foreach (Category cat in restaurant.Categories)
                    {
                        cat.isDeleted = true;
                        foreach (Food f in cat.Foods)
                        {
                            f.isDeleted = true;
                        }
                    }
                
            }
            _context.Restaurants.Update(restaurant);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(Guid id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
