using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext DB;
        public OrderController(ApplicationDbContext db)
        {
            this.DB= db;
        }
        [HttpGet]
        public async Task<ActionResult> Index(string searchString)
        {
            var or = DB.Orders.Include(o => o.Customer)
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if(! string.IsNullOrWhiteSpace(searchString))
            {
                or = or.Where(o => o.OrderStatus.Contains(searchString) || o.Customer!.CustomerName.Contains(searchString));
            }
            ViewBag.SearchString = searchString;    
            return View(await or.OrderByDescending(o => o.OrderDate).ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null )
            {
                return NotFound();
            }
            var or = await DB.Orders.Include(o => o.Customer)
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (or == null)
            {
                return NotFound();
            }
            return View(or);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.Customers = await DB.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            ViewBag.Products = await DB.Products
                .Where(p => p.IsActive && p.Quantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int CustomerId, int ProductId,int Quantity,string? Notes)
        {
            if(CustomerId <= 0)
            {
                ModelState.AddModelError("CustomerId", "Please select a Customer.");
            }
            if(ProductId <= 0)
            {
                ModelState.AddModelError("ProductId", "Please Select a Product.");
            }
            if(Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
            }
            var Product = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId);
            if (Product == null)
            {
                ModelState.AddModelError("ProductId", "Product not found.");

            }
            else if (Quantity > Product.Quantity)
            {
                ModelState.AddModelError("Quantity", "Requested quantity is greater than available stock.");
            }
            var Cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == CustomerId && c.IsActive);
            if(Cust == null)
            {
                ModelState.AddModelError("CustomerId", "Customer not found.");
            }
            if(!ModelState.IsValid)
            {
                ViewBag.Customers = await DB. Customers.Where(c=> c.IsActive)
                    .OrderBy(c => c.CustomerName)
                    .ToListAsync();

                ViewBag.Products = await DB.Products
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();
                return View();
            }
            decimal totalPrice = Product!.Price * Quantity;
            var order = new Order
            {
                CustomerId = CustomerId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalAmount = totalPrice,
                Notes = Notes
            };
            DB.Orders.Add(order);
            await DB.SaveChangesAsync();
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = ProductId,
                Quantity = Quantity,
                UnitPrice = Product.Price,
                TotalPrice = totalPrice,
            };
            DB.OrderItems.Add(orderItem);
            Product.Quantity -= Quantity;
            DB.Products.Update(Product);
            await DB.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<ActionResult>Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var order = await DB.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();

            }
            ViewBag.Customers = await DB.Customers
                .Where (c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(int id, int CustomerId,string OrderStatus, string? Notes )
        {
            var order = await DB.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();

            }
            var Cust = await DB.Customers.FirstOrDefaultAsync(c => c.CustomerId == CustomerId && c.IsActive);
            if(Cust == null)
            {
                ModelState.AddModelError("CustomerId", "Please select a valid customer.");
                ViewBag.Customers = await DB.Customers
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CustomerName)
                    .ToListAsync();
                return View(order);
            }
            order.CustomerId = CustomerId;
            order.OrderStatus = OrderStatus;
            order.Notes = Notes;
            DB.Orders.Update(order);
            await DB.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var or = await DB.Orders.Include(o => o.Customer).Include(o => o.orderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (or == null)
            {
                return NotFound();
            }
            return View(or);
        }
        [HttpPost,ActionName ("Delete")]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> DeleteConfirmed(int id)
        {
            var or = await DB.Orders.Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (or == null)
            {
                return NotFound();
            }
            foreach (var item in or.orderItems)
            {
                var Pro = await DB.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (Pro != null)
                {
                 Pro.Quantity += item.Quantity;
                }
            }
            DB.OrderItems.RemoveRange(or.orderItems);
            DB.Orders.Remove(or);
            await DB.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

    }
}
