using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;

namespace ProductManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext DB;
        public DashboardController(ApplicationDbContext db)
        {
            this .DB = db;
        }
        [HttpGet]
        public async Task<ActionResult>Index()
        {
            ViewBag.TotalCategorie = await DB.Categories.CountAsync();
            ViewBag.TotalCustomer = await DB.Customers.CountAsync();
            ViewBag.TotalOrder = await DB.Orders.CountAsync();
            ViewBag.TotalProducts = await DB.Products.CountAsync();
            ViewBag.TotalSuppliers = await DB.Suppliers.CountAsync();
            ViewBag.RecentOrder = await DB.Orders.Include(o => o.Customer).OrderByDescending(o => o.OrderDate).Take(10).ToListAsync();
            ViewBag.LowStockProducts = await DB.Products.Include(p => p.Category).Where(p => p.IsActive && p.Quantity <= 10).OrderBy(p => p.Quantity).Take(10).ToListAsync();
            ViewBag.TotalStock = await DB.Products.Select(p => (int?)p.Quantity).SumAsync() ?? 0;
            ViewBag.TotalSales = await DB.Orders.Select(o => (decimal?)o.TotalAmount).SumAsync() ?? 0;
            return View();
            
           
        }
    }
}
