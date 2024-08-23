using Figgle;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace AsciiArtSvc
{
    public static class AsciiArt
    {
        public static string Write(string text, string? fontName = null)
        {
            FiggleFont? font = null;
            if (!string.IsNullOrWhiteSpace(fontName))
            {
                font = typeof(FiggleFonts).GetProperty(fontName, BindingFlags.Static | BindingFlags.Public)?
                    .GetValue(null) as FiggleFont;
            }

            font ??= FiggleFonts.Standard;
            return font.Render(text);
        }

        public static Lazy<IEnumerable<string>> AllFonts = new(() =>
        from p in typeof(FiggleFonts).GetProperties(BindingFlags.Public | BindingFlags.Static) select p.Name);

        public static Lazy<IEnumerable<(string Name, FiggleFont Font)>> AllFontsParam = new(() => from p in typeof(FiggleFonts)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        select (
            Name: p.Name,
            Font: (p.GetValue(null) as FiggleFont)
        ));
    }
}
