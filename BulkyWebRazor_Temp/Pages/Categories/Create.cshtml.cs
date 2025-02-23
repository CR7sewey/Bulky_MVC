using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        [BindProperty] // on post binds and available on post handler
        public Category Category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        
        public IActionResult OnPost()
        {


            
                if (Category.CategoryName.ToLower() == Category.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("", "The category name can't be the same as Display Order");
                }
                // if the category exists in the database, add an error
                var objFromDb = _db.Categories.FirstOrDefault(s => s.CategoryName == Category.CategoryName);
                if (objFromDb != null)
                {
                    ModelState.AddModelError("CategoryName", "The category already exists");
                }

                if (Category.DisplayOrder <= 0)
                {
                    ModelState.AddModelError("DisplayOrder", "The value needs to be positive");
                }
                if (ModelState.IsValid)
                {
                _db.Categories.Add(Category); // provided and habndled by entity framework! no open and close connection, or inserts!
                _db.SaveChanges();
                //TempData.Add("Success", $"{objFromDb.CategoryName} created successfully");
                TempData["Success"] = $"{Category.CategoryName} created successfully";
                return RedirectToPage("Index");
                }
            return Page();
        }

    }
}
