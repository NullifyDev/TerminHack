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

            string flags = "", dir = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-"))
                    flags += s[1..^0];
                break;
            }
            
            Content? n = tfs.Retreive(terminal, dir ?? ".");
            switch (n)
            {
                case null: 
                    terminal.Error(this, dir, "no such file or directory");
                    return;

                case string file:
                    terminal.Print($"└─ {tfs.PathStr}/{file}");
                    return;

                case Dictionary<string, Content> folder:
                    // terminal.Print(tfs.PathStr);
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