using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository: IRepository<Category>
    {
        // methods from IRepository are inherited
        void Update(CategoryRepository category);
        void Save();

    }
}
