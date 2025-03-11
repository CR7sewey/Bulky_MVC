using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            // manual updating
            /*var objFromDB = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            var imageURL = objFromDB.ImageUrl;
            if (objFromDB != null)
            {
                objFromDB.Title = product.Title
            ...
                if (product.ImageUrl == null) {
                    objFromDB.ImageUrl = imageURL;
                }
               */

            _db.Products.Update(product);
        }
    }
    
 
}
