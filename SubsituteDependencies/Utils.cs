using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsituteDependencies
{
    public static class Utils
    {
        public static string? FindFirstMatchingLine(Stream stream, string searchText)
        {
            var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    return line;
                }
            }
            return null;
        }
    }
}