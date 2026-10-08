using ProductManagementSystem.Models;

namespace ProductManagementSystem.Services.Interfaces
{
    public interface IReportService
    {
        Task<Dictionary<string, object>> GetDashboardReportAsync(); 
        Task<List<Product>> GetStockReportAsync();
        Task<List<Order>> GetSalesReportAsync();
        Task<List<Customer>> GetCustomersReportAsync();
        Task<List<Supplier>> GetSuppliersReportAsync();
        Task<List<Product>> GetLowStockProductAsync();
    }
}
