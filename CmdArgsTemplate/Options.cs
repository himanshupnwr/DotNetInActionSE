using CommandLine;

namespace CmdArgsTemplate
{
    internal record Options
    {
        [Value(0, Required = true, HelpText = "Some text")]
        public string? Text { get; init; }
    }
}
