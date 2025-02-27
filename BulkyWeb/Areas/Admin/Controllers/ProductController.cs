using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
                .Select(i => new SelectListItem  // projections!!!!!!!! similar to map
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                });
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            // viewdata/viewbag to pass data to view (not vice versa) to temporarily store data not in a model (in this case Product)

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CategoryList
            };
            if (id != null) // update
            {
                Product obj = _unitOfWork.Product.Get(i => i.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                productVM.Product = obj;
            }         

            
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["Success"] = $"{obj.Product.Title} created successfully";
                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
               .Select(i => new SelectListItem  // projections!!!!!!!! similar to map
               {
                   Text = i.CategoryName,
                   Value = i.Id.ToString()
               });
            ProductVM productVM = new ProductVM()
            {
                Product = obj.Product,
                CategoryList = CategoryList
            };

            return View(productVM);
        }

        /*public IActionResult Edit(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
                .Select(i => new SelectListItem  // projections!!!!!!!! similar to map
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                });

            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _unitOfWork.Product.Get(i => i.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            ProductVM productVM = new ProductVM()
            {
                Product = obj,
                CategoryList = CategoryList
            };

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj.Product);
                _unitOfWork.Save();
                TempData["Success"] = $"{obj.Product.Title} updated successfully";

                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
               .Select(i => new SelectListItem  // projections!!!!!!!! similar to map
               {
                   Text = i.CategoryName,
                   Value = i.Id.ToString()
               });
            
            obj.CategoryList = CategoryList;
            return View(obj);
        }
        */
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _unitOfWork.Product.Get(i => i.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _unitOfWork.Product.Get(i => i.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["Error"] = $"{obj.Title} deleted successfully";

            return RedirectToAction("Index");

        }

    }
}
