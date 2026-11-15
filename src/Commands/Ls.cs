namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class Ls : ICommands
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

            string flags = "", dir = "", pwd = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-"))
                    flags += s[1..^0];

                if (dir == "") dir = (s ?? "");
                break;
            }

            object? n = tfs.Traverse(terminal, dir);
            switch (n)
            {
                case null: return;
                case string file:
                    pwd = $"{tfs.PathStr}/{dir}";
                    terminal.Print(pwd != "/"
                        ? pwd[Convert.ToInt32(pwd.StartsWith("//"))..^(Convert.ToInt32(pwd.EndsWith("/")))]
                        : "/"
                    );
                    return;
                case Dictionary<string, Content> folder:
                    if (tfs.PathStr != "/")
                    {
                        pwd = $"{tfs.PathStr}/{dir}";
                        pwd = pwd[Convert.ToInt32(pwd.StartsWith("//"))..^(Convert.ToInt32(pwd.EndsWith("/")))];
                    }
                    else pwd = $"/{dir}";

                    terminal.Print(pwd);
                    for (int i = 0; i < folder.Count; i++)
                    {
                        string c = folder.ElementAt(i).Key ?? "TH_INTERNAL_ERROR_NULL_ENTRY";
                        terminal.Print(i < folder.Count - 1
                            ? $"├─ {c}"
                            : $"└─ {c}"
                        );
                    }
                    return;

                default:
                    terminal.Error(this, dir, $"INTERNAL ERROR: unknown type {n.GetType().FullName}");
                    return;
            }
        }
    }
}