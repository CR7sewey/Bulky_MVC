using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        private readonly ApplicationDbContext _db;
        
        public CategoryRepository(ApplicationDbContext db): base(db) // reponsavel pela injecao da db
        {
            _db = db;
            
        }

       /* public void Save()
        {
            _db.SaveChanges();
        }
       */
        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
