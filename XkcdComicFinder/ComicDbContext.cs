using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkcdComicFinder
{
    public class ComicDbContext : DbContext
    {
        public DbSet<Comic> Comics { get; set; } = null!;

        public ComicDbContext(DbContextOptions options): base (options) { }
    }
}
