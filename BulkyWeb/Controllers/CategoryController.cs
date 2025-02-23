using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {

        // if legacy .NET framework, we need to open DB connection here, then query the database, then close the connection
        // if .NET Core, we use Entity Framework Core
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList<Category>();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.CategoryName.ToLower() == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("", "The category name can't be the same as Display Order");
            }
            // if the category exists in the database, add an error
            var objFromDb = _db.Categories.FirstOrDefault(s => s.CategoryName == obj.CategoryName);
            if (objFromDb != null)
            {
                ModelState.AddModelError("CategoryName", "The category already exists");
            }
            
            if (obj.DisplayOrder <= 0)
            {
                ModelState.AddModelError("DisplayOrder", "The value needs to be positive");
            }


            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj); // provided and habndled by entity framework! no open and close connection, or inserts!
                _db.SaveChanges();
                //TempData.Add("Success", $"{objFromDb.CategoryName} created successfully");
                TempData["Success"] = $"{obj.CategoryName} created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? obj = _db.Categories.Find(id);
            // FirstOrDefault(s => s.Id == id);
            Category? obj = _db.Categories.FirstOrDefault(i => i.Id == id);
            // where(s => s.Id == id).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.CategoryName.ToLower() == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("", "The category name can't be the same as Display Order");
            }


            if (obj.DisplayOrder <= 0)
            {
                ModelState.AddModelError("DisplayOrder", "The value needs to be positive");
            }


            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj); // provided and habndled by entity framework! no open and close connection, or inserts!
                _db.SaveChanges();
               // TempData.Add("Success", $"{obj.CategoryName} category updated successfully");
                TempData["Success"] = $"{obj.CategoryName} updated successfully";

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? obj = _db.Categories.Find(id);
            // FirstOrDefault(s => s.Id == id);
            Category? obj = _db.Categories.FirstOrDefault(i => i.Id == id);
            // where(s => s.Id == id).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var objFromDb = _db.Categories.FirstOrDefault(s => s.Id == id);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(objFromDb);
            _db.SaveChanges();
            //TempData.Add("Success", $"{objFromDb.CategoryName} deleted successfully");
            TempData["Error"] = $"{objFromDb.CategoryName} deleted successfully";

            return RedirectToAction("Index");
        }

    }
}
