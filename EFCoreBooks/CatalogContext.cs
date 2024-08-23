using Microsoft.EntityFrameworkCore;

namespace EFCoreBooks
{
    internal class CatalogContext : DbContext
    {
        //The DbContext object is a lightweight object that isn’t meant to operate as a singleton;
        //it’s meant to be created and disposed per request
        public DbSet<Book> Books { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseInMemoryDatabase("ManningBooks");

        public static void SeedBooks()
        {
            using var dbContext = new CatalogContext();
            if (!dbContext.Database.EnsureCreated())
            {
                return;
            }

            dbContext.Add(new Book("Grokking Simplicity"));
            dbContext.Add(new Book("API Design Patterns"));
            var efBook = new Book("EF Core in Action");
            efBook.Ratings.Add(new Rating { Comment = "Great!" });
            efBook.Ratings.Add(new Rating { Stars = 4 });
            dbContext.Add(efBook);
            dbContext.SaveChanges();
        }

        public static async Task WriteBookToConsoleAsync(string title)
        {
            using var dbContext = new CatalogContext();
            var book = await dbContext.Books.Include(b => b.Ratings).FirstOrDefaultAsync(b => b.Title == title);
            if (book == null)
            {
                Console.WriteLine(@$"""{title}"" not found.");
            }
            else
            {
                Console.WriteLine(@$"Book ""{book.Title}"" has id {book.Id}");
                book.Ratings.ForEach(r =>Console.WriteLine($"\t{r.Stars} stars: {r.Comment ?? "-blank-"}"));
            }
        }
    }
}
