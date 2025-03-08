using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    //[Authorize(Roles = SD.Role_User_Company)]
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            Company obj = new Company();
            if (id != null) // update
            {
                obj = _unitOfWork.Company.Get(i => i.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
            }
            
            return View(obj);
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = $"{obj.Name} created successfully";
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
            Company obj = _unitOfWork.Company.Get(i => i.Id == id);
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
            Company obj = _unitOfWork.Company.Get(i => i.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }


        #region API endpoint
        [HttpGet]
        public IActionResult Get()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return Json( new { data = companies} );
        }

        [HttpDelete]
        public IActionResult DeleteCompany(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Company not found..." });
            }
            Company obj = _unitOfWork.Company.Get(i => i.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Company not found..." });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Company deleted..." });
        }

        #endregion
    }
}
