namespace TerminHack;

using static FileSystem;

public partial class Command
{
    public class Touch : ICommands
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
            string name = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (name == "") name = s;
                break;
            }

            var sstr = name.Split('/');
            if (sstr.Length <= 1 && name == "")
            {
                terminal.Print(@"touch: missing file operand
Try 'touch --help' for more information.");
                return;
            }
            
            object? parent = sstr.Count() > 1
                ? tfs.Retreive(terminal, string.Join("/", sstr[0..^1]))
                : tfs.CurrentFolder;

            string child = sstr.Count() >= 1
                ? sstr[sstr.Length-1]
                : sstr[0];

            switch (parent)
            {
                case null: return;
                case string file:
                    terminal.Error(this, name, "not a directory");
                    return;

                case Dictionary<string, Content> folder:
                    folder.Add(child, "");
                    tfs.WriteSave();
                    return;
            }
        }
    }
}