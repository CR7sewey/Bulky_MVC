using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IProductRepository: IRepository<Product>
    {
        // methods from IRepository are inherited
        void Update(Product product);
        //void Save();
    }
}
