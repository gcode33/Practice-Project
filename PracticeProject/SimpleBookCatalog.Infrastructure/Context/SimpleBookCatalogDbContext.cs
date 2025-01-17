using Microsoft.EntityFrameworkCore;
using SimpleBookCatalog.Domain.Entities;

namespace SimpleBookCatalog.Infrastructure.Context
{
    public class SimpleBookCatalogDbContext : DbContext
    {
        public SimpleBookCatalogDbContext(DbContextOptions<SimpleBookCatalogDbContext> options)
            : base(options)
        {
            Console.WriteLine("DbContext constructed with options");
        }

        public DbSet<Books> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            Console.WriteLine($"OnConfiguring called. IsConfigured: {optionsBuilder.IsConfigured}");
        }
    }
}
