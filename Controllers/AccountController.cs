using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext DB;
        public AccountController(ApplicationDbContext db)
        {
            DB = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string FullName, string Email,string Password,string ConfirmPassword)
        {
            if(string.IsNullOrWhiteSpace(FullName))
            {
                ModelState.AddModelError("FullName", "Full Name is required.");
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not Match.");
            }
            if (await DB.Users.AnyAsync(u => u.Email == Email))
            {
                ModelState.AddModelError("Email", "Email is already Registered.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = Password,
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.Now,
            };
            DB.Users.Add(user);
            await DB.SaveChangesAsync();
            TempData["SuccessMessage"] = "Registration successful. Please Login.";
            return RedirectToAction(nameof(Login));
           
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string Email, string Password)
        {
            if(string .IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.ErrorMessage = "Email and Password are Required.";
                return View();
            }
            var passwordHash = Password;
            var user = await DB.Users.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password && u.IsActive);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid Email or Password.";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(
                    ClaimTypes.Name,
                    user.FullName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)

            };
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
            return RedirectToAction("Index", "Dashboard");
        }
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }
        private static string HashPassword(string Password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}
