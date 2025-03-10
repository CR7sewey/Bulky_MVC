﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository: IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
        IEnumerable<ShoppingCart> GetAllUser(string Id, string? includeProperties = null);
    }
}
