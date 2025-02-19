using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    
    public class Category
    {
        [Key]
        public int Id { get; set; } // Primary Key - CategoryId
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
    }
}
