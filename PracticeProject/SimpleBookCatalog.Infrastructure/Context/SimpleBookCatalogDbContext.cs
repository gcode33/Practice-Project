using Microsoft.EntityFrameworkCore;
using PracticeProject.Entities;

namespace SimpleBookCatalog.Infrastructure.Context
{
    public class SimpleBookCatalogDbContext : DbContext
    {

        public SimpleBookCatalogDbContext(DbContextOptions<SimpleBookCatalogDbContext> options) : base(options) 
        {
        
        }

        public DbSet<Books> Books {  get; set; }
    }
}
