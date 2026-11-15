namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class Pwd : ICommands
    {
        public string GetManual()
          => @"
-1 -          One
-2 -          Two
-3<4>         Three<four>
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args)
            => terminal.Print(terminal.fs.PathStr);
    }
}