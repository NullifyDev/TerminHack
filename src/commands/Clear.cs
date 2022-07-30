using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerminHack;

namespace TerminHack.Commands
{
    public class Clear : ICommandHandler
    {
        public string[] arguments = new string[0];
        public bool AppliesTo(string command)
        {
            command.ToLower();
            return command == "clear";
        }   

        public void Handle(string[] arguments)
        {
            Console.Clear();
        }
    }
}
