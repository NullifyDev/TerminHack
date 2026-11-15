namespace TerminHack;

// using Newtonsoft.Json.Bson;
using static TerminHack.FileSystem;

public partial class Command
{
    public class Bash : ICommands
    {
        public void Run(Terminal terminal, ReadOnlySpan<string> args) {}
        public string GetManual() => "\"This is literally a dummy command so \"bash: cd: missing_folder: no such file or directory\" can work.\"\n\t\t  - Developer";
    }
}