using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext DB;
        public ProductController (ApplicationDbContext db)
        {
            this.DB = db;
        }
        [HttpGet]
        public async Task<ActionResult> Index(string searchString)
        {
            var Pro = DB.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                Pro = Pro.Where(p => p.ProductName.Contains(searchString));
            }
            ViewBag.SearchString = searchString; ;
            return View(await Pro.OrderByDescending(p => p.ProductId)
                .ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Pro = await DB.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (Pro == null)
            {
                return NotFound();
            }
            return View(Pro);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.Categories = await DB.Categories
                .Where(c => c.IsAction)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.Suppliers = await DB.Suppliers
                .Where(s => s.IsActive)
                .OrderBy (s =>  s.SupplierName)
                .ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Create (Product pro)
        {
            if(ModelState.IsValid)
            {
                pro.CreatedDate = DateTime.Now;
                pro.IsActive = true;
                DB.Products.Add(pro);
                await DB.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await DB.Categories
                .Where(c => c.IsAction)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
            ViewBag.Suppliers = await DB.Suppliers
                .Where (s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync ();
            return View(pro);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Pro = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (Pro == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await DB.Categories
                .Where(c => c.IsAction)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
            ViewBag.Suppliers = await DB.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
            return View(Pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Product pro)
        {
            if (id != pro.ProductId)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    DB.Update(pro);
                    await DB.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!DB.Products.Any(p => p.ProductId == pro.ProductId))
                    {
                        return NotFound();
                    }
                    throw;
                        
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await DB.Categories
                .Where(c => c.IsAction)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
            ViewBag.Suppliers = await DB.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
            return View(pro);
        }
        [HttpGet]
        public async Task <ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var pro = await DB.Products.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.ProductId == id);

            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var pro = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (pro != null)
            {
                DB.Products.Remove(pro);
                await DB.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
