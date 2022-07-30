using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerminHack.Commands
{
    public class Echo : ICommandHandler
    {
        public string[] arguments = new string[0];
        public bool AppliesTo(string command)
        {
            command.ToLower();
            return command == "echo";
        }

        public void Handle(string[] arguments)
        {

            string line = "";
            for (int i = 0; i < arguments.Length; i++) {
                if (i == arguments.Length) { line += arguments[i]; } else { line += $"{arguments[i]} "; }
            }
            Utils.Terminal.Output(line, 1);
        }
    }
}
