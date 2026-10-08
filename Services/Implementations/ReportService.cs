using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Interfaces;

namespace ProductManagementSystem.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext DB;
        public ReportService(ApplicationDbContext db)
        {
            this.DB = db;
        }
        public async Task<Dictionary<string, object>> GetDashboardReportAsync()
        {
            var rep = new Dictionary<string, object>();
            rep["TotalProducts"] = await DB.Products.CountAsync();
            rep["ActiveProducts"] = await DB.Products.CountAsync(p => p.IsActive);
            rep["InactiveProduct"] = await DB.Products.CountAsync(p => !p.IsActive);
            rep["TotalCategories"] = await DB.Categories.CountAsync();
            rep["TotalCustomers"] = await DB.Customers.CountAsync();
            rep["ActiveCustomers"] = await DB.Customers.CountAsync(c => c.IsActive);
            rep["TotalSuppliers"] = await DB.Suppliers.CountAsync();
            rep["ActiveSuppliers"] = await DB.Suppliers.CountAsync(s => s.IsActive);
            rep["TotalOrders"] = await DB.Orders.CountAsync();
            rep["PendingOrders"] = await DB.Orders.CountAsync(o => o.OrderStatus == "Pending");
            rep["CompleteOrders"] = await DB.Orders.CountAsync(o => o.OrderStatus == "Completed");
            rep["CancelledOrders"] = await DB.Orders.CountAsync(o => o.OrderStatus == "Cancelled");
            rep["TotalStock"] = await DB.Products.SumAsync(p => p.Quantity);
            rep["LowStockProducts"] = await DB.Products.CountAsync(p => p.Quantity <= 10 && p.IsActive);
            rep["TotalSales"] = await DB.Orders.Where(o => o.OrderStatus != "Cancelled").SumAsync(o => o.TotalAmount);
            var Today = DateTime.Today;
            rep["TodaySales"] = await DB.Orders.Where(o => o.OrderDate.Date == Today && o.OrderStatus == "Cancelled").SumAsync(o => o.TotalAmount);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            rep["MonthIySales"] = await DB.Orders.Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear && o.OrderStatus == "Cancelled").SumAsync(o => o.TotalAmount);
            return rep;
        }
        public async Task<List<Product>> GetStockReportAsync()
        {
         return await DB.Products.Include(p => p.Category).Include(p => p.Supplier).OrderBy(p => p.Quantity).ToListAsync();
          
        }
        public async Task<List<Order>> GetSalesReportAsync()
        {
            return await DB.Orders.Include(o => o.Customer).Include(o => o.orderItems)
                .ThenInclude(oi => oi.Product).OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersReportAsync()
        {
          return await DB.Customers.Include(c => c.Orders).OrderBy(c => c.CustomerName).ToListAsync();
        }

        public async Task<List<Supplier>> GetSuppliersReportAsync()
        {
          return await DB.Suppliers.Include(s => s.Products).OrderBy(s => s.SupplierName).ToListAsync();
        }

        public async Task<List<Product>> GetLowStockProductAsync()
        {
            return await DB.Products.Include(p => p.Category).Include(p => p.Supplier)
                .Where(p => p.Quantity <= 10 && p.IsActive).OrderBy(p => p.Quantity).ToListAsync();
        }



    }
}
