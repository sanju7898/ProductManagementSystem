using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext DB;
        public CustomerController(ApplicationDbContext dB)
        {
            this.DB = dB;
        }

        public async Task< ActionResult> Index(string SearchString)
        {
            var cust = DB.Customers.AsQueryable();
            if(!string.IsNullOrWhiteSpace(SearchString))
            {
                cust = cust.Where(c => c.CustomerName.Contains(SearchString) || c.Phone.Contains(SearchString) || (c.Email != null && c.Email.Contains(SearchString)));
            }
            ViewBag.SearchString = SearchString;
            return View(await cust.ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (cust == null)
            {
                return NotFound();
            }
            return View(cust);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Create(Customer cust)
        {
            if (ModelState.IsValid)
            {
                cust.CreatedDate = DateTime.Now;
                cust.IsActive = true;
                DB.Customers.Add(cust);
                await DB.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cust);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var Cust = await DB.Customers.FindAsync(id);
            if (Cust == null)
            {
                return NotFound();
            }
            return View(Cust);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Customer cust)
        {
            if (id != cust.CustomerId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    DB.Update(cust);
                    await DB.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException )
                {
                    if(!DB.Customers.Any(c => c.CustomerId == cust.CustomerId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cust);
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if( id  == null)
            {
                return NotFound();
            }
            var Cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            
                if (Cust == null)
                {
                    return NotFound();
                }
                return View(Cust);
            
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> DeleteConfirmed(int? id)
        {
            var cust = await DB.Customers.FindAsync(id);
            if( cust != null)
            {
                DB.Customers.Remove(cust);
                await DB.SaveChangesAsync();
            }
            return RedirectToAction(nameof (Index));
        }
    }
}
