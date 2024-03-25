using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TabGıda.Data;
using TabGıda.Models;

namespace TabGıda.Controllers
{
    [Authorize(Policy ="AdminAccess")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Companies;
            return View(await applicationDbContext.ToListAsync());
        }

        

        // GET: Company/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostalCode,Address,Phone,EmailAddress,TaxNumber,WebAdress,Id,Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.Id = Guid.NewGuid();
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(company);
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PostalCode,Address,Phone,EmailAddress,TaxNumber,WebAdress,Id,Name,isActive")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            
            return View(company);
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
              .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {

            Company? company = _context.Companies!.Where(c => c.Id == id).Include(c => c.Users).Include(r => r.Restaurants).ThenInclude(c => c.Categories).ThenInclude(f => f.Foods).FirstOrDefault();
            if (company != null)
            {
                company.isDeleted = true;
                foreach (User u in company.Users)
                {
                    u.isDeleted = true;
                }
                foreach (Restaurant rest in company.Restaurants)
                {
                    rest.isDeleted = true;
                    foreach (Category cat in rest.Categories)
                    {
                        cat.isDeleted = true;
                        foreach (Food f in cat.Foods)
                        {
                            f.isDeleted = true;
                        }
                    }
                }
            }
            _context.Companies.Update(company);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(Guid id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
