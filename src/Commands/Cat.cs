namespace TerminHack;

using System.Formats.Asn1;
using static TerminHack.FileSystem;

public partial class Command
{
    public class Cat : ICommands
    {
        public string GetManual()
          => @"-1 -          One
-2 -          Two
-3<4>         Three<four>
";

        public void Run(Terminal terminal, ReadOnlySpan<string> args)
        {
            var tfs = terminal.fs;
            string file = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (file == "") file = s;
                break;
            }

            string[] ss = file.Split("/");
            var ret = tfs.Retreive(terminal, ss.Length > 1 ? string.Join("/", ss[0..]) : ss[0], new(tfs));

            if (!ret.HasValue) 
                terminal.Error(this, file, "No such file or Directory");

            switch (ret)
            {
                case string s:
                    terminal.Print(s);
                    break;

                case Dictionary<string, Content> d:
                    terminal.Error(this, file, "is a directory");
                    break;

                default: 
                    terminal.Error(this, file, "No such file or Directory");
                    break;
            }
        }
    }
}
