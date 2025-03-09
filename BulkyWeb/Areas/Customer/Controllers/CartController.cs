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
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = ShoppingCartList,
                OrderTotal = 0
            };

            // Nota: Criei ViewModel pq para a view quero passar a lista de produtos e o total do pedido

            return View(shoppingCartVM);
        }
    }
}
