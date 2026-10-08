using ProductManagementSystem.Models;

namespace ProductManagementSystem.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync(string? searchString);
        Task<Order> GetOrderByIdAsync(int id);
        Task <List<Customer>> GetCustomersAsync();
        Task<List<Product>> GetProductsAsync();
        Task<bool> CreateOrderAsync(int customerId, int ProductId, int Quantity, string? notes);
        Task<bool> UpdateOrderAsync(int id, int CustomerId, string orderStatus, string? notes);
        Task<bool> DeleteOrderAsync(int id);
    }
}
