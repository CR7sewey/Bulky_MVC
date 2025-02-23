using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty] // on post binds and available on post handler
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null && id!=0)
            {
                Category = _db.Categories.Find(id);
            }
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
                _db.Categories.Update(Category); // provided and habndled by entity framework! no open and close connection, or inserts!
                _db.SaveChanges();
                // TempData.Add("Success", $"{obj.CategoryName} category updated successfully");
                TempData["Success"] = $"{Category.CategoryName} updated successfully";

                return RedirectToPage("Index");
            }
            return Page();
        }

    }
}
