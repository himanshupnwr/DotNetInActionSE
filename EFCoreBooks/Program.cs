namespace EFCoreBooks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var dbContext = new CatalogContext();
            CatalogContext.SeedBooks();

            var userRequests = new[] {
                 ".NET in Action",
                 "Grokking Simplicity",
                 "API Design Patterns",
                 "EF Core in Action"
            };

            //await and async don’t move Tasks to background threads, but Task.Run does

            var tasks = new List<Task>();
            foreach (var userRequest in userRequests)
            {
                tasks.Add(CatalogContext.WriteBookToConsoleAsync(userRequest));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var book in dbContext.Books.OrderBy(b => b.Id))
            {
                Console.WriteLine($"\"{book.Title}\" has id {book.Id}");
            }
        }
    }
}
