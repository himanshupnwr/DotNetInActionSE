using BooksAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI
{
    public class CatalogContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;

        public CatalogContext(DbContextOptions options) :
          base(options)
        { }
    }
}
