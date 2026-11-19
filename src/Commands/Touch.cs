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
            string dir = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (dir == "") dir = s;
                break;
            }

            var sstr = dir.Split("/");
            var name = sstr[^0];
            Content? parent = tfs.CurrentFolder;
            if (sstr.Length > 1)
                parent = tfs.Retreive(terminal, sstr.Length > 2 ? string.Join("/",  sstr[0..^1]) : sstr[0], new(tfs));

            switch (parent)
            {
                case string file:
                    terminal.Error(this, dir, "File exists");
                    return;

                case Dictionary<string, Content> folder:
                    folder.Add(name, "");
                    tfs.WriteSave();
                    return;

                case null:
                    terminal.Print("mkdir: No such file or directory");
                    return;
            }
        }
    }
}