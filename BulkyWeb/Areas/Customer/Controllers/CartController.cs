using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; } // automatically ppouplated with values bcs of BindProperty
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);

            IEnumerable<ShoppingCart> ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(it => it.UserId == userID, includeProperties: "Product");
            double ot = ShoppingCartList.Select(it => {

                double price = GetCartPrice(it);
                it.Price = price;

                return it.Price*it.Count;

            }).Sum();


            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = ShoppingCartList,
                OrderHeader = new OrderHeader()
                {
                    ApplicationUser = user,
                    ApplicationUserId = userID,
                    OrderTotal = ot
                },
                
            };

            // Nota: Criei ViewModel pq para a view quero passar a lista de produtos e o total do pedido

            return View(ShoppingCartVM);
        }

        public ActionResult Plus(int cartId, int productId)
        {
            Console.WriteLine(cartId);
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(it => it.Id == cartId && it.ProductId == productId);
            shoppingCart.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public ActionResult Minus(int cartId, int productId) // productId not needed
        {
            Console.WriteLine(cartId);
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(it => it.Id == cartId && it.ProductId == productId, tracked: true);
            if (shoppingCart.Count > 1)
            {
                shoppingCart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
            }
           
            _unitOfWork.Save();

            
            var valueCount = 0;
            valueCount = _unitOfWork.ShoppingCart.GetAll(it => it.ApplicationUser.Id == shoppingCart.UserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.SessionCart, valueCount);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int cartId, int productId)
        {
            Console.WriteLine(cartId);
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(it => it.Id == cartId && it.ProductId == productId, tracked: true); // otherwise remove will fail bcs not tracked! -> repository

         
            var valueCount = 0;
            valueCount = _unitOfWork.ShoppingCart.GetAll(it => it.ApplicationUser.Id == shoppingCart.UserId).ToList().Count()-1;
            HttpContext.Session.SetInt32(SD.SessionCart, valueCount);

            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();


            

            return RedirectToAction("Index");
        }

        public ActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);

            IEnumerable<ShoppingCart> ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(it => it.UserId == userID, includeProperties: "Product");
            double ot = ShoppingCartList.Select(it => {

                double price = GetCartPrice(it);
                it.Price = price;

                return it.Price * it.Count;

            }).Sum();


            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = ShoppingCartList,
                OrderHeader = new OrderHeader()
                {
                    ApplicationUser = user,
                    ApplicationUserId = userID,
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber,
                    OrderTotal = ot
                },

            };
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public ActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(it => it.UserId == userID, includeProperties: "Product"); // automatically populated (BindProperty)

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ApplicationUser appuser = user; // do not do HsoppingCartVM....ApplicationUser bcs it will create a new orw in the table/entity; add a navigation property...
            ShoppingCartVM.OrderHeader.ApplicationUserId = userID;

            double ot = ShoppingCartVM.ShoppingCartList.Select(it => {

                double price = GetCartPrice(it);
                it.Price = price;

                return it.Price * it.Count;

            }).Sum();

            ShoppingCartVM.OrderHeader.OrderTotal = ot;

            if (appuser.CompanyId.GetValueOrDefault()==0)
            {
                // regular costumer and need to capture payment
                ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Pending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus_Pending;
            }
            else
            {
                // Company user
                ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Approved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus_DelayedPayment;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetails = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Save();
            }

            // talvez aqui deveria-se tirar as coisas do carinho de compras 
            if (appuser.CompanyId.GetValueOrDefault() == 0)
            {
                // regular costumer and need to capture payment - PAYMENT - STRIPE
                var DOMAIN = "https://localhost:7169/";
                var options = new Stripe.Checkout.SessionCreateOptions
                    {
                        SuccessUrl = DOMAIN+$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                        CancelUrl = DOMAIN + $"customer/cart/index",
                        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                foreach (var items in ShoppingCartVM.ShoppingCartList)
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
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                //ShoppingCartVM.OrderHeader.PaymentIntendId = session.PaymentIntentId; -- only not null after payment done!
                //ShoppingCartVM.OrderHeader.SessionId = session.Id;
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303); // redirect to new url
            }
            else
            {
                // Company user - CONIFMRATION
                return RedirectToAction("OrderConfirmation", new {id = ShoppingCartVM.OrderHeader.Id});
            }
        }

        public ActionResult OrderConfirmation(int? Id)
        {

            // check if payment was successful
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(it => it.Id == Id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatus_DelayedPayment)
            { 
                // order by customer
                var sessionID = orderHeader.SessionId;

                var service = new SessionService();
                var serviceSession = service.Get(sessionID);
                if (serviceSession.PaymentIntentId.ToLower() == "paid")
                {
                    // update Payment Intent Id
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, sessionID, serviceSession.PaymentIntentId);
                    // update status
                    _unitOfWork.OrderHeader.UpdateStatus(ShoppingCartVM.OrderHeader.Id, SD.Status_Approved, SD.PaymentStatus_Approved);
                    _unitOfWork.Save();
                    

                    // payment was successful
                }
                else
                {
                    // payment was not successful
                }



            }

            _unitOfWork.ShoppingCart.RemoveRange(_unitOfWork.ShoppingCart.GetAll(it => it.UserId == orderHeader.ApplicationUserId).ToList());
            _unitOfWork.Save();




            return View(Id);
        }


        private double GetCartPrice(ShoppingCart cart)
        {
             
                if (cart.Count < 50)
                {
                    return cart.Product.Price;
                }
                else if (cart.Count >= 50 && cart.Count < 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                return cart.Product.Price100;
            }

              

        }
    }
}
