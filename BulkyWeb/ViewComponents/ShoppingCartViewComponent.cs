using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (userID != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    var valueCount = 0;
                    valueCount = _unitOfWork.ShoppingCart.GetAll(it => it.ApplicationUser.Id == userID.Value).ToList().Count();
                    HttpContext.Session.SetInt32(SD.SessionCart, valueCount);
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
                    } 

    }
}
