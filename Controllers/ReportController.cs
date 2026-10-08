using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;

namespace ProductManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext DB;
        public ReportController (ApplicationDbContext db)
        {
            this.DB= db;
        }
        [HttpGet]
        public async Task<ActionResult>Index()
        {
            ViewBag.TotalProducts = await DB.Products.CountAsync();
            ViewBag.ActionProducts = await DB.Products.CountAsync(p => p.IsActive);
            ViewBag.InactiveProducts = await DB.Products.CountAsync(p => !p.IsActive);
            ViewBag.TotalStock = await DB.Products
               .SumAsync(p => p.Quantity);

            ViewBag.LowStockProducts = await DB.Products
                .CountAsync(p => p.Quantity <= 10);

            // Category Report
            ViewBag.TotalCategories = await DB.Categories
                .CountAsync();

            // Customer Report
            ViewBag.TotalCustomers = await DB.Customers
                .CountAsync();

            ViewBag.ActiveCustomers = await DB.Customers
                .CountAsync(c => c.IsActive);

            // Supplier Report
            ViewBag.TotalSuppliers = await DB.Suppliers
                .CountAsync();

            ViewBag.ActiveSuppliers = await DB.Suppliers
                .CountAsync(s => s.IsActive);

            // Order Report
            ViewBag.TotalOrders = await DB.Orders
                .CountAsync();

            ViewBag.PendingOrders = await DB.Orders
                .CountAsync(o => o.OrderStatus == "Pending");

            ViewBag.CompletedOrders = await DB.Orders
                .CountAsync(o => o.OrderStatus == "Completed");

            ViewBag.CancelledOrders = await DB.Orders
                .CountAsync(o => o.OrderStatus == "Cancelled");

            // Sales Report
            ViewBag.TotalSales = await DB.Orders
                .Where(o => o.OrderStatus != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            // Today's Sales
            var today = DateTime.Today;

            ViewBag.TodaySales = await DB.Orders
                .Where(o =>
                    o.OrderDate.Date == today &&
                    o.OrderStatus != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            // This Month Sales
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            ViewBag.MonthlySales = await DB.Orders
                .Where(o =>
                    o.OrderDate.Month == currentMonth &&
                    o.OrderDate.Year == currentYear &&
                    o.OrderStatus != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            return View();
        }

        // Product Stock Report
        [HttpGet]
        public async Task<IActionResult> StockReport()
        {
            var products = await DB.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.Quantity)
                .ToListAsync();

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> SalesReport()
        {
            var orders = await DB.Orders
                .Include(o => o.Customer)
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Customer Report
        [HttpGet]
        public async Task<IActionResult> CustomerReport()
        {
            var customers = await DB.Customers
                .Include(c => c.Orders)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return View(customers);
        }

        // Supplier Report
        [HttpGet]
        public async Task<IActionResult> SupplierReport()
        {
            var suppliers = await DB.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return View(suppliers);
        }

        // Low Stock Report
        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var products = await DB.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Quantity <= 10 && p.IsActive)
                .OrderBy(p => p.Quantity)
                .ToListAsync();

            return View(products);
        }

    }
}

