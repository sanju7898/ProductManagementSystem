using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryId {  get; set; }
        
        [Required]
        [StringLength (100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(300)]
        public string? CategoryDescription { get; set; }
        public bool IsAction { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<Product>? Products { get; set; } = new List<Product>();

    }
}
