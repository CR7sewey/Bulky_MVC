using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)] // server side validation
        public string Title { get; set; }
        [MaxLength(2000)] // server side validation
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 1000.")] // server side validation
        public double ListPrice { get; set; }


        [Required]
        [Display(Name = "Price 1-50")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 1000.")] // server side validation
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price 50+")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 1000.")] // server side validation
        public double Price50 { get; set; }


        [Required]
        [Display(Name = "Price 100+")]
        [Range(1, 1000, ErrorMessage = "The value must be between 1 and 1000.")] // server side validation
        public double Price100 { get; set; }


        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }


    }
}
