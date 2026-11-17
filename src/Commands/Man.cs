namespace TerminHack;

public partial class Command
{
    public class Man : ICommands
    {
        public string GetManual()
          => @"
-1 -          One
-2 -          Two
-3<4>         Three<four>
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args)
        {
            if (!terminal.cmdHnd.commands.ContainsKey(args[0]))
            {
                terminal.Print($"No manual entry for {args[0]}");
                return;
            }
            
            terminal.Print((
                (ICommands)Activator.CreateInstance(
                    terminal.cmdHnd.commands[args[0]]
                )!
            ).GetManual());
        }
    }
}