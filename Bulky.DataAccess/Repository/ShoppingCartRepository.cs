using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.Update(shoppingCart);
        }

        public IEnumerable<ShoppingCart> GetAllUser(string Id, string? includeProperties = null)
        {
            if (Id != null)
            {
                return _db.ShoppingCarts.Where(it => it.UserId == Id ).ToList();
            }
            return base.GetAll(it => it.UserId == Id, includeProperties);
        }
    }
}
