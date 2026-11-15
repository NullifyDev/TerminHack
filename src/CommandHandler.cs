using System.Reflection;
using System.Runtime.InteropServices;

namespace TerminHack;

public partial class Command;

public class CommandHandler
{
    public Dictionary<string, Type> commands = typeof(Command).GetNestedTypes(BindingFlags.Public)
        .ToDictionary(t => t.Name.ToLower());

    public void Submit(Terminal terminal, Span<string> command)
    {
        string cmd = command[0];
        switch(cmd) {
            case "": return;
            case "history": break;
            default:
                terminal.History.Add(string.Join(" ", command.ToArray()));
                break;
        }

        if (commands.ContainsKey(cmd))
        {
            Type commandType = commands[cmd];
            ICommands commandInstance = (ICommands)Activator.CreateInstance(commandType)!;
            commandInstance.Run(terminal, new ReadOnlySpan<string>(command.ToArray()[1..^0]));
            return;
        }
        terminal.Print($"bash: {cmd}: command not found");
    }
}