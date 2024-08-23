using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithJson
{
    public record Joke(int Id, string Type, string Setup, string Punchline)
    { }
}
