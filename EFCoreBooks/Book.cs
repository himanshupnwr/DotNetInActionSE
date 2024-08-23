
using System.ComponentModel.DataAnnotations;

namespace EFCoreBooks
{
    internal class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Rating> Ratings { get; } = new();
        public Book(string title)
        {
            Title = title;
        }
    }

    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; } = 5;

        public string? Comment { get; set; }
    }
}
