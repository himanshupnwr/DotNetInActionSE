using CommandLine;
using Figgle;
using System.Reflection;

namespace DotNetInActionConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: HelloDotnet <text>");
                Environment.Exit(1);
            }
            //AsciiArt.Write(args[0]);
            //Console.WriteLine(FiggleFonts.Standard.Render(args[0]));

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(AsciiArt.Write)
                .WithNotParsed(_ => Console.WriteLine("Usage: HelloDotnet <text> --font Big"));
        }
    }
    public static class AsciiArt
    {
        public static void Write(Options option)
        {
            FiggleFont? font = null;
            if (!string.IsNullOrWhiteSpace(option.Font))
            {
                font = typeof(FiggleFonts)
                    .GetProperty(option.Font,BindingFlags.Static | BindingFlags.Public)
                    ?.GetValue(null)as FiggleFont;
                if (font == null)
                {
                    Console.WriteLine($"Could not find font '{option.Font}'");
                }
            }

            font ??= FiggleFonts.Standard;

            if (option?.Text != null)
            {
                Console.WriteLine(font.Render(option.Text));
                Console.WriteLine($"Brought to you by {typeof(AsciiArt).FullName}");
            }
        }
    }
}
