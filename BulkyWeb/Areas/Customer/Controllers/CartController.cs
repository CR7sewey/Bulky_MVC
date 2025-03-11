using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
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


            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = ShoppingCartList,
                OrderTotal = ot
            };

            // Nota: Criei ViewModel pq para a view quero passar a lista de produtos e o total do pedido

            return View(shoppingCartVM);
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
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(it => it.Id == cartId && it.ProductId == productId);
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
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int cartId, int productId)
        {
            Console.WriteLine(cartId);
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(it => it.Id == cartId && it.ProductId == productId);
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public ActionResult Summary()
        {
            return View();
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
