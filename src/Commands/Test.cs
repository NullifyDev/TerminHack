using System.Collections;

namespace TerminHack;

public partial class Command
{
    public class Test : ICommands
    {
        public string GetManual()
          => @"Nothing more, Nothing less - Keep it simple, cut the excess.
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args)
        {
        }
    }
}