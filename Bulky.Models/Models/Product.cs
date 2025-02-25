using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)] // server side validation
        public string Title { get; set; }
        [MaxLength(200)] // server side validation
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }


        [Required]
        [Display(Name = "List Price 1-50")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 10000.")] // server side validation
        public List<double> Price { get; set; }

        [Required]
        [Display(Name = "List Price 50+")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 10000.")] // server side validation
        public List<double> Price50 { get; set; }


        [Required]
        [Display(Name = "List Price 100+")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 10000.")] // server side validation
        public List<double> Price100 { get; set; }



    }
}
