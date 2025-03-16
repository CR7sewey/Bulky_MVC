using System.Diagnostics;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller // order controller to manage orders
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API calls
        // https://datatables.net/manual/ajax
        // endpoint to get all products - api
        [HttpGet]
        public IActionResult GetAll(string status)
        {
        
            List<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList(); 
            Dictionary<string,string> keyValuePairs = new Dictionary<string, string>()
            {
                {"pending", SD.Status_Pending},
                {"inprocess", SD.Status_InProcess},
                {"completed", SD.Status_Shipped},
                {"approved", SD.Status_Approved},                
            };
            if (status != "all" && status != null)
            {
                orders = orders.Where(o => o.OrderStatus == keyValuePairs[status]).ToList();
            }
            return Json(new { data = orders });
            
            
        }



        #endregion
    }
}
