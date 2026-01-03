using CMS_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

       public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }


		public DbSet<User> Users { get; set; }
	}
}
