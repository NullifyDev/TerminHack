namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class Echo : ICommands
    {
        public string GetManual()
          => @"
-1 -          One
-2 -          Two
-3<4>         Three<four>
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args) 
        {
            if (args[0] == "--debug") {
                terminal.Print("ECHO: DEBUG!");
                args = args[1..^0];
                System.Console.Write($"[{args.Length}] ");
            }

            foreach(var a in args) 
                System.Console.Write($"{a} ");

            terminal.Print();
        }
    }
}