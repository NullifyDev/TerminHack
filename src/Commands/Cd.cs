using System.Collections;

namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class Cd : ICommands
    {
        public string GetManual()
          => @"
-1 -          One
-2 -          Two
-3<4>         Three<four>
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args)
        {
            var tfs = terminal.fs;
            string dir = "";

            foreach(var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (dir == "") dir = s;
                break;
            }

            switch(tfs.Retreive(terminal, dir))
            {
                case string:
                    terminal.Error(this, dir, "not a directory");
                    return;

                case Dictionary<string, Content> folder:
                    var d = dir.Split("/");
                    tfs.EnterDirectory(terminal, d[d.Length-1]);
                    tfs.UpdatePath();
                    return;

                case null:
                    terminal.Error(this, dir, "no such file or directory");
                    return;
            }
        }
    }
}