using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkcdComicFinder
{
    public interface IComicRepository
    {
        Task<int> GetLatestNumberAsync();
        Task AddComicAsync(Comic comic);
        IAsyncEnumerable<Comic> Find(string searchText);
    }
}
