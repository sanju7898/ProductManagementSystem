using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Interfaces;

namespace ProductManagementSystem.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext DB;
        public OrderService(ApplicationDbContext db)
        {
           this.DB = db;
        }
        public async Task<List<Order>> GetAllOrdersAsync(string? searchString)
        {
            var or = DB.Orders.Include(o => o.Customer).Include(o => o.orderItems).ThenInclude(oi => oi.Product).AsQueryable();
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                or = or.Where(o => o.OrderStatus.Contains(searchString) || o.Customer!.CustomerName.Contains(searchString));
            }
            return await or.OrderByDescending(o => o.OrderDate).ToListAsync();
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await DB.Orders.Include(o => o.Customer).Include(o => o.orderItems).ThenInclude(oi => oi.Product)
                  .FirstOrDefaultAsync(o => o.OrderId == id);
        }
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await DB.Customers.Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName).ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
           return await DB.Products.Where(p => p.IsActive && p.Quantity > 0)
                .OrderBy(p => p.ProductName).ToListAsync();
        }

        public async Task<bool> CreateOrderAsync(int customerId, int ProductId, int Quantity, string? notes)
        {
            if (customerId <= 0|| ProductId <= 0 || Quantity <= 0)
            {
                return false;
            }
            var Cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);
            if( Cust == null )
            {
                return false;
            }
            var product = await DB.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == ProductId &&
                    p.IsActive);

            if (product == null)
            {
                return false;
            }

            if (Quantity > product.Quantity)
            {
                return false;
            }

            decimal totalPrice = product.Price * Quantity;

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalAmount = totalPrice,
                Notes = notes
            };

            DB.Orders.Add(order);

            await DB.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = ProductId,
                Quantity = Quantity,
                UnitPrice = product.Price,
                TotalPrice = totalPrice
            };

            DB.OrderItems.Add(orderItem);

            // Reduce Stock
            product.Quantity -= Quantity;

            await DB.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> UpdateOrderAsync(int id, int CustomerId, string orderStatus, string? notes)
        {
            var or = await DB.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (or == null)
            {
                return false;
            }
            var cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == CustomerId && c.IsActive);
            if (cust == null)
            {
                return false;
            }
            or.CustomerId = CustomerId;
            or.OrderStatus = orderStatus;
            or.Notes = notes;
            await DB.SaveChangesAsync();
            return true;
           

        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
           var or = await DB.Orders.Include(o => o.orderItems).FirstOrDefaultAsync(o => o.OrderId == id);
            if( or == null)
            {
                return false;
            }
            foreach(var item in or.orderItems)
            {
                var pro = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (pro == null)
                {
                    pro.Quantity += item.Quantity;
                }
            }
            DB.OrderItems.RemoveRange(or.orderItems);
            DB.Orders.Remove(or);
            await DB.SaveChangesAsync();
            return true;
        }

    }

}
