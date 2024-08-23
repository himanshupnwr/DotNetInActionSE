using CommandLine.Text;
using CommandLine;

namespace CmdArgsTemplate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var results = Parser.Default.ParseArguments<Options>(args).
                WithParsed<Options>(options => { Console.WriteLine(options.Text);});

            results.WithNotParsed(_ => Console.WriteLine(HelpText.RenderUsageText(results)));
        }
    }
}
