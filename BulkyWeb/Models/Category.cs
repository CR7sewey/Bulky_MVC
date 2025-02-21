using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    
    public class Category
    {
        [Key]
        public int Id { get; set; } // Primary Key - CategoryId
        [Required]
        [MaxLength(30)] // server side validation
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "The value must be between 1 and 100.")] // server side validation
        public int DisplayOrder { get; set; }
    }
}
