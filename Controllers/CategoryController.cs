using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext DB;
        public CategoryController(ApplicationDbContext db)
        {
            this.DB = db;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public async Task<ActionResult> Index(string SearchString)
        {
            var cate = DB.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchString)) 
            {
                cate = cate.Where(c => c.CategoryName.Contains(SearchString));
            }
            ViewBag.SerachString = SearchString;
            return View(await cate.ToListAsync());
        }
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = await DB.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category cate)
        {
            if( ModelState.IsValid )
            {
                cate.CreatedDate = DateTime.Now;
                cate.IsAction = true;
                DB.Categories.Add(cate);
                await DB.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var cate = await DB.Categories.FindAsync(id);
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Category cate)
        {
            if (id != cate.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCategory = await DB.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.CategoryName = cate.CategoryName;
                existingCategory.CategoryDescription = cate.CategoryDescription;
                existingCategory.IsAction = cate.IsAction;

                await DB.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(cate);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = await DB.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var cate = await DB.Categories.FindAsync(id);
            if (cate != null)
            {
                DB.Categories.Remove(cate);
                await DB.SaveChangesAsync();
            }
            return RedirectToAction(nameof (Index));
        }

            
    }
}
