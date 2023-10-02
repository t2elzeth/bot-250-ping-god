using CommandLine;

namespace DbSeeds;

public sealed class Arguments
{
    [Option("env", Required = true, HelpText = "Set current environment")]
    public string Environment { get; set; } = null!;
}