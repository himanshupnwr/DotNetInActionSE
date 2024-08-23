using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkcdComicFinder
{
    public interface IXkcdClient
    {
        Task<Comic> GetLatestAsync();

        Task<Comic?> GetByNumberAsync(int number);
    } 
}
