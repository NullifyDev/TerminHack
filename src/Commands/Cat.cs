namespace TerminHack;

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
            string p = string.Join("/", ss.Length > 1
                    ? ss[0..^1]
                    : ss[0]),

                    c = ss.Count() >= 1
                        ? ss[ss.Length-1]
                        : ss[0];

            switch (tfs.Traverse(terminal, p)?[ss[ss.Length-1]])
            {
                case string s:
                    terminal.Print(s);
                    break;

                case Dictionary<string, Content> d:
                    terminal.Error(this, file, "is a directory");
                    break;

                case null:
                default: 
                    terminal.Error(this, file, "No such file or Directory");
                    break;
            }

            // if (f!.ToString().Length > 1) terminal.Print(f.Contents.ToString());
            // else System.Console.WriteLine();
        }
    }
}
