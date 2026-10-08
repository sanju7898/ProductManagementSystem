using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Interfaces;

namespace ProductManagementSystem.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext DB;
        public ProductService(ApplicationDbContext db)
        {
            this.DB = db;
        }
        public async Task<List<Product>> GetAllProductsAsync(string? searchString)
        {
            var pro = DB.Products.Include(p => p.Category)
                .Include(p => p.Supplier).AsQueryable();
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                pro = pro.Where(p => p.ProductName.Contains(searchString));
            }
            return await pro.OrderByDescending(p => p.ProductId).ToListAsync();
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await DB.Products.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.ProductId == id);

        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
           return await DB.Categories.Where(c => c.IsAction).OrderBy(c => c.CategoryName).ToListAsync();
        }
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
           return await DB.Suppliers.Where(s => s.IsActive).OrderBy(s=> s.SupplierName).ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product pro)
        {
            pro.CreatedDate = DateTime.Now; 
            pro.IsActive = true;
            DB.Products.Add(pro);
            await DB.SaveChangesAsync();
            return pro;
        }
        public async Task<bool> UpdateProductAsync(Product pro)
        {
           var existingProduct = DB.Products.FirstOrDefault(p => p.ProductId == pro.ProductId);
            if (existingProduct == null)
            {
                return false;
            }
            existingProduct.ProductName = pro.ProductName;
            existingProduct.CategoryId = pro.CategoryId;
            existingProduct.Price = pro.Price;
            existingProduct.Quantity = pro.Quantity;
            existingProduct.Description = pro.Description;
            existingProduct.SupplierId = pro.SupplierId;
            existingProduct.IsActive = pro.IsActive;
            await DB.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var Pro = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if( Pro == null)
            {
                return false;
            }
            DB.Products.Remove(Pro);
            await DB.SaveChangesAsync();
            return true;
        }
    }
}
