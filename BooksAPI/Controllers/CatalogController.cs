using BooksAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Controllers
{
    //most important vulnerabilities in asp.net applications
    //JavaScript Object Notation (JSON) processing
    //Data flowing across HTTP request/response, which an attacker might sniff or tamper with
    //Potential SQL injection vulnerability for SQL database
    //Weak database access control
    //Elevation using impersonation
    //Weak authentication scheme
    //Weak web-service access control

    [ApiController]
    [Route("[controller]")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _dbContext;

        public CatalogController(
          CatalogContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize("AuthenticatedUsers")]
        public IAsyncEnumerable<Book> GetBooks(string? titleFilter = null)
        {
            IQueryable<Book> query = _dbContext.Books.Include(b => b.Ratings);
            if (titleFilter != null)
            {
                query = query.Where(b => b.Title.ToLower().Contains(titleFilter.ToLower()));
            }

            return query.AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        [Authorize("AuthenticatedUsers")]
        public Task<Book?> GetBook(int id)
        {
            return _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        }

        [HttpPost]
        [Authorize("OnlyMe")]
        public async Task<Book> CreateBook(
          BookCreateCommand command)
        {
            var book = new Book(
              command.Title,
              command.Description
            );

            var entity = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return entity.Entity;
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize("OnlyMe")]
        public async Task<IActionResult> UpdateBook(int id, BookUpdateCommand command)
        {
            var book = await _dbContext.FindAsync<Book>(id);
            if (book == null)
            {
                return NotFound();
            }

            if (command.Title != null)
            {
                book.Title = command.Title;
            }

            if (command.Description != null)
            {
                book.Description = command.Description;
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize("OnlyMe")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _dbContext.Books
              .Include(b => b.Ratings)
              .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            _dbContext.Remove(book);
            var rows = await _dbContext.SaveChangesAsync();
            Console.WriteLine("Rows deleted: " + rows);
            return NoContent();
        }

        public record BookCreateCommand(string Title, string? Description) { }
        public record BookUpdateCommand(string? Title, string? Description) { }
    }
}
