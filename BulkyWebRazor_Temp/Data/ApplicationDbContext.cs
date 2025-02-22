using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext: DbContext // root class of entity framework
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // seed data
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, CategoryName = "Action", DisplayOrder = 1 });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 2, CategoryName = "SciFi", DisplayOrder = 2 });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 3, CategoryName = "History", DisplayOrder = 3 });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 4, CategoryName = "Drama", DisplayOrder = 4 });

        }
    }
}
