using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerminHack;

namespace TerminHack.Commands
{
    public class Shutdown : ICommandHandler
    {
        public string[] arguments = new string[0];
        public bool AppliesTo(string command)
        {
            command.ToLower();
            return command == "shutdown";
        }

        public void Handle(string[] arguments)
        {
            if (arguments[0] == "now") { Program.Shutdown();} else { Program.Shutdown(Int32.Parse(arguments[0])*1000); }
        }

    }
}
