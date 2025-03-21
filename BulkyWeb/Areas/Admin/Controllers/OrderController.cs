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
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == OrderVM.OrderHeader.Id);
            
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;


            if (orderHeader.Carrier.IsNullOrEmpty())
            {
                TempData["Error"] = "Please enter the carrier name";
                return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

            }
            else if (orderHeader.TrackingNumber.IsNullOrEmpty())
            {
                TempData["Error"] = "Please enter the tracking number";
                return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
            }


            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.OrderStatus = SD.Status_Shipped;
            if (orderHeader.PaymentStatus == SD.PaymentStatus_DelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.update(orderHeader);
            _unitOfWork.Save();

            //_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Status_Shipped);
            //_unitOfWork.Save();
            TempData["Success"] = "Order details updated successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
//            var orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == OrderVM.OrderHeader.Id);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Status_InProcess);  
            _unitOfWork.Save();
            TempData["Success"] = "Order details updated successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == OrderVM.OrderHeader.Id);


            // stripe refund
            if (orderHeader.PaymentStatus == SD.PaymentStatus_Approved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.PaymentIntendId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                //orderHeader.OrderStatus = SD.Status_Refunded;
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Status_Cancelled, SD.Status_Refunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Status_Cancelled, SD.Status_Cancelled);

            }
            _unitOfWork.Save();
            TempData["Success"] = "Order details updated successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetails = _unitOfWork.OrderDetails.GetAll(it => it.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            var DOMAIN = "https://localhost:7169/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = DOMAIN + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = DOMAIN + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var items in OrderVM.OrderDetails)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(items.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = items.Product.Title,
                            }
                        },
                        Quantity = items.Count,
                    }
                    );
            }

            var service = new SessionService();
            // create session
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            //ShoppingCartVM.OrderHeader.PaymentIntendId = session.PaymentIntentId; -- only not null after payment done!
            //ShoppingCartVM.OrderHeader.SessionId = session.Id;
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); // redirect to new url

        }



        public ActionResult PaymentConfirmation(int orderHeaderId)
        {

            // check if payment was successful
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == orderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatus_DelayedPayment)
            {
                // order by customer
                var sessionID = orderHeader.SessionId;

                var service = new SessionService();
                var serviceSession = service.Get(sessionID);
                if (serviceSession.PaymentIntentId.ToLower() == "paid")
                {
                    // update Payment Intent Id
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, sessionID, serviceSession.PaymentIntentId);
                    // update status
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus ,SD.PaymentStatus_Approved);
                    _unitOfWork.Save();


                    // payment was successful
                }
                else
                {
                    // payment was not successful
                }



            }


            return View(orderHeaderId);
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


            //role
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);
                orders = orders.Where(o => o.ApplicationUserId == userID).ToList();
            }

            return Json(new { data = orders });
            
            
        }
        #endregion
    }
}
