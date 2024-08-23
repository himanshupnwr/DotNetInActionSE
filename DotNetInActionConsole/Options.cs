using CommandLine;

namespace DotNetInActionConsole
{
    public record Options
    {
        [Value(0, Required = true)]
        public string? Text { get; init; }

        [Option('f', "font")]
        public string? Font { get; init; }
    }
}
