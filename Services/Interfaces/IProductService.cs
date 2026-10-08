using ProductManagementSystem.Models;

namespace ProductManagementSystem.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>>GetAllProductsAsync(string? searchString);
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Supplier>> GetSuppliersAsync();
        Task<Product> CreateProductAsync(Product pro);
        Task<bool> UpdateProductAsync(Product pro);
        Task<bool> DeleteProductAsync(int id);
    }
}
