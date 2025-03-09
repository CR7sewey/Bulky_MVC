using System.Diagnostics;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int id)
        {

            Product product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "Category");

            if (product == null || id == 0)
            {
                return NotFound();
            }
           
            ShoppingCart shoppingCart = new ShoppingCart();
                //shoppingCart.UserId = userID;
            shoppingCart.Count = 1;
            shoppingCart.ProductId = product.Id;
            shoppingCart.Product = product;
 
            return View(shoppingCart);

        }

        [HttpPost]
        [Authorize] // all the roles, just logged in
        public IActionResult Details(ShoppingCart? sc)
        {


            Product product = _unitOfWork.Product.Get(p => p.Id == sc.Id, includeProperties: "Category");
              
            if (product == null)
            {
                return NotFound();
            }

            /*ShoppingCart dafaultShoppingCart = new ShoppingCart();
            //shoppingCart.UserId = userID;
            dafaultShoppingCart.Count = 1;
            dafaultShoppingCart.ProductId = product.Id;
            dafaultShoppingCart.Product = product;*/

            // Steps to add to shopping cart
            // 1. Get the user ID
            // 2. Get the shopping cart for the user
            // 3. If the user does not have a shopping cart, create a new shopping cart
            // 4. If the user already has a shopping cart but not the producted added, update the shopping cart (Product)
            // 5. If the user already has a shopping cart but the producted added, update the shopping cart (Count)

            // 1. Get the user ID
            if (!User.Identity.IsAuthenticated)
            {
               // return Redirect("/Identity/Account/Login");
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(p => p.Id == userID);


            // 2. Get the shopping cart for the user
            //IEnumerable<ShoppingCart> shoppingCartUser = _unitOfWork.ShoppingCart.GetAllUser(userID);
            ////ShoppingCart shoppingCartUser = _unitOfWork.ShoppingCart.Get(t => t.UserId == userID && t.ProductId == product.Id);
            //// 3. If the user does not have a shopping cart, create a new shopping cart
            //if (shoppingCartUser == null)
            //{
            //    ShoppingCart shoppingCart = new ShoppingCart
            //    {
            //        //shoppingCart.UserId = userID;
            //        Count = sc.Count,
            //        ProductId = product.Id,
            //        Product = product,
            //        ApplicationUser = user
            //    };
            //    _unitOfWork.ShoppingCart.Add(shoppingCart);
            //    _unitOfWork.Save();
            //    TempData["Success"] = $"{product.Title} added successfully and cart created";
            //    return RedirectToAction(nameof(Index));
            //}
            //else
            //{
            //    // 4. If the user already has a shopping cart but not the producted added, update the shopping cart (Product)
            //    ShoppingCart shoppingCartProduct = shoppingCartUser.Where(it => it.ProductId == product.Id).FirstOrDefault();
            //    if (shoppingCartProduct == null)
            //    {
            //        ShoppingCart shoppingCart = new ShoppingCart
            //        {
            //            ProductId = product.Id,
            //            Product = product,
            //            ApplicationUser = user,
            //            Count = sc.Count,
            //        };
            //        _unitOfWork.ShoppingCart.Add(shoppingCart);
            //        _unitOfWork.Save();
            //        TempData["Success"] = $"{product.Title} added successfully";
            //        return RedirectToAction(nameof(Index));
            //    }
            //    // 5. 
            //    else
            //    {
            //        shoppingCartProduct.Count += sc.Count;
            //        _unitOfWork.ShoppingCart.Update(shoppingCartProduct);
            //        _unitOfWork.Save();
            //        TempData["Success"] = $"{product.Title} increased successfully";
            //        return RedirectToAction(nameof(Index));
            //    }
            //}

            ShoppingCart shoppingCartUser = _unitOfWork.ShoppingCart.Get(t => t.UserId == userID && t.ProductId == product.Id);
            // 3. If the user does not have a shopping cart, create a new shopping cart
            if (shoppingCartUser == null)
            {
                ShoppingCart shoppingCart = new ShoppingCart
                {
                    //shoppingCart.UserId = userID;
                    Count = sc.Count,
                    ProductId = product.Id,
                    Product = product,
                    ApplicationUser = user
                };
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                TempData["Success"] = $"{product.Title} added successfully and cart created";
                
            }
            else
            {

                shoppingCartUser.Count += sc.Count; 
                _unitOfWork.ShoppingCart.Update(shoppingCartUser); // not needed
                _unitOfWork.Save();
                TempData["Success"] = $"{product.Title} increased successfully";
                
            }
            return RedirectToAction(nameof(Index));
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
