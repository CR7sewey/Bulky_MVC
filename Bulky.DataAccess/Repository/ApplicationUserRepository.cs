using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        // ApplicationUser to use it later
        private readonly ApplicationDbContext _db;
        
        public ApplicationUserRepository(ApplicationDbContext db): base(db) // reponsavel pela injecao da db
        {
            _db = db;
            
        }
    }
}
