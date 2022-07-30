using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using TerminHack;
using System.Text;

namespace TerminHack
{
    public static class CommandHandler
    {
        private static readonly Dictionary<ICommandHandler, string> _commandHandlers = new() {
            { new Commands.Echo(), "echo" }, 
            { new Commands.Shutdown(), "shutdown"},
            { new Commands.Clear(), "clear" },
            
            /* { new Commands.Ls(), "ls"},
            { new Commands.Rm(), "rm" },
            { new Commands.RmDir(), "rmdir" },
            { new Commands.mkdir(), "mkdir" }, */
        };

        public static void HandleCommand(string command, string[] arguments)
        {
            command.ToLower();
            if (_commandHandlers.Values.Contains(command)) _commandHandlers.SingleOrDefault(x => x.Value == (command)).Key.Handle(arguments); 
            else Utils.Terminal.Error($"Unknown Command: {command}");
        }
    }
}