using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    
    public class Category
    {
        [Key]
        public int Id { get; set; } // Primary Key - CategoryId
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}
