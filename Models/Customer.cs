using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$",
     ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string ? Address { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
