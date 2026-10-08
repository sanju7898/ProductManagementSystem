using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.VisualBasic;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext DB;
        public SupplierController(ApplicationDbContext db)
        {
            this.DB = db;
        }
        [HttpGet]
        public async Task<ActionResult> Index(string searchString)
        {
            var sup = DB.Suppliers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                sup = sup.Where(s => s.SupplierName.Contains(searchString) || s.Phone.Contains(searchString) || (s.Email != null && s.Email.Contains(searchString)));


            }
            ViewBag.SearchString = searchString;
            return View(await sup.OrderByDescending(s => s.SupplierId).ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sup = await DB.Suppliers.Include(s => s.Products).FirstOrDefaultAsync(s => s.SupplierId == id);
            if (sup == null)
            {
                return NotFound();
            }
            return View(sup);

        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Supplier sup)
        {
            if (ModelState.IsValid)
            {
                sup.CreatedDate = DateTime.Now;
                sup.IsActive = true;
                DB.Suppliers.Add(sup);
                await DB.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sup);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var sup = await DB.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (sup == null)
            {
                return NotFound();
            }
            return View(sup);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(int id, Supplier sup )
        {
            if( id != sup.SupplierId)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    DB.Update(sup);
                    await DB.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!DB.Suppliers.Any(s => s.SupplierId==sup.SupplierId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sup);
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var sup = await DB.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if( sup == null)
            {
                return NotFound();
            }
            return View(sup);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>DeleteConfirmed(int id)
        {
            var sup = await DB.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (sup != null)
            {
                DB.Suppliers.Remove(sup);
                await DB.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
