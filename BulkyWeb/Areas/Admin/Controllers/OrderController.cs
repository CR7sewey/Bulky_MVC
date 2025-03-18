﻿using System.Diagnostics;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller // order controller to manage orders
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            // order header
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);
            //role
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
              
            }
            else {
                if (userID == orderHeader.ApplicationUserId)
                {
                    return RedirectToAction("Index");
                }
            }

            

            if (orderHeader == null)
            {
                return NotFound();
            }


            // order details
            IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == orderHeader.Id, includeProperties: "Product");
            //

            OrderVM = new()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {

            // automatically updated by the model binder - OrderVM.OrderHeader = orderHeader;
            var orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == OrderVM.OrderHeader.Id);
            orderHeader.Name = OrderVM.OrderHeader.Name;
            orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeader.City = OrderVM.OrderHeader.City;
            orderHeader.State = OrderVM.OrderHeader.State;
            orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            }

            _unitOfWork.OrderHeader.update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order details updated successfully";

            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
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
