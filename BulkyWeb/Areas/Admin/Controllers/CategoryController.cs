using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {

        // if legacy .NET framework, we need to open DB connection here, then query the database, then close the connection
        // if .NET Core, we use Entity Framework Core
        // remove the DB connection from here to Repository!
        //private readonly ICategoryRepository _db;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            // _db = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //  List<Category> objCategoryList = _db.Categories.ToList<Category>();
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
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
            //var objFromDb = _db.Categories.FirstOrDefault(s => s.CategoryName == obj.CategoryName);
            var objFromDb = _unitOfWork.Category.Get(s => s.CategoryName == obj.CategoryName);
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
                //_db.Categories.Add(obj); // provided and habndled by entity framework! no open and close connection, or inserts!
                _unitOfWork.Category.Add(obj);
                //_db.SaveChanges();
                _unitOfWork.Save();
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
            //Category? obj = _db.Categories.FirstOrDefault(i => i.Id == id);
            Category? obj = _unitOfWork.Category.Get(i => i.Id == id);
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
                // _db.Categories.Update(obj); // provided and habndled by entity framework! no open and close connection, or inserts!
                _unitOfWork.Category.Update(obj);
                //_db.SaveChanges();
                _unitOfWork.Save();
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
            //Category? obj = _db.Categories.FirstOrDefault(i => i.Id == id);
            Category? obj = _unitOfWork.Category.Get(i => i.Id == id);
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
            // var objFromDb = _db.Categories.FirstOrDefault(s => s.Id == id);
            var objFromDb = _unitOfWork.Category.Get(s => s.Id == id);
            if (objFromDb == null)
            {
                return NotFound();
            }
            //_db.Categories.Remove(objFromDb);
            _unitOfWork.Category.Remove(objFromDb);
            //_db.SaveChanges();
            _unitOfWork.Save();
            //TempData.Add("Success", $"{objFromDb.CategoryName} deleted successfully");
            TempData["Error"] = $"{objFromDb.CategoryName} deleted successfully";

            return RedirectToAction("Index");
        }

    }
}
