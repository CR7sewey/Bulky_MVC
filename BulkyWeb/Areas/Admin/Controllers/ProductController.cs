using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
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
                if (file != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/product");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (file == null)
                    {
                        obj.Product.ImageUrl = _unitOfWork.Product.Get(i => i.Id == obj.Product.Id).ImageUrl;
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                        {
                            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.Product.ImageUrl.TrimStart('@').TrimStart('\\'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    }
              //  C: \Users\Utilizador\C#\source\NET_CORE_MVC\Bulky\BulkyWeb\wwwroot\images\product\6e54ddd0-d445-4650-a8cd-6e8a1472ad07.jpeg

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyToAsync(fileStream);
                    }

                    // Return the file path or URL
                    string fileUrl = $"@/images/product/{uniqueFileName}";
                    obj.Product.ImageUrl = fileUrl;
                }
                if (obj.Product.Id != 0) // present in the database - update
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
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

        #region API calls
        // https://datatables.net/manual/ajax
        // endpoint to get all products - api
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new {success = false, message = "Product not found..."});
            }
            Product obj = _unitOfWork.Product.Get(i => i.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Product not found..." });
            }
            var fileName = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('@').TrimStart('\\'));

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();

            if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                
            

            return Json(new { success = true, message = "Product deleted..." } );
        }

        #endregion


        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "BadRequest";
            }

            try
            {
                // Ensure the directory exists
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/product");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return the file path or URL
                string fileUrl = $"@/images/product/{uniqueFileName}";
                return fileUrl;
            }
            catch (Exception ex)
            {
                return "Internal Server Error: {ex.Message}";
            }
        }

    }




}
