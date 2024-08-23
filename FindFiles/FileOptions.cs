using CommandLine;

namespace FindFiles
{
    public record FileOptions
    {
        [Value(0, Required = true, HelpText = "Text to search for")]
        public string? Text { get; init; }
  
        [Value(1, Required = false, HelpText = "File to search")]
        public string? Filename { get; init; }
    }
}
