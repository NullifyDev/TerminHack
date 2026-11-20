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
            bool recursive = false, force = false;


            foreach(var s in args)
            {
                if (s.StartsWith("-")) 
                    continue;

                if (dir == "") dir = s;
                break;
            }

            string[] p = dir.Split("/");
            string name = p.Last();

            Content? parent = tfs.CurrentFolder;
            if (p.Length > 1)
                parent = p.Length > 1 ? tfs.Retreive(terminal, p.Length > 2 ? string.Join("/",  p[0..^1]) : p[0], new(tfs)) : new Content(tfs.CurrentFolder);

            switch (parent)
            {
                case string file:
                    break;

                case Dictionary<string, Content> folder:
                    if (!folder.ContainsKey(name)) 
                    {
                        folder.Add(name, new Content(""));
                        tfs.WriteSave();
                    }
                    break;

                case null:
                    terminal.Error(this, $"Cannot touch '{dir}'", "No such file or directory");
                    break;
            }
        }
    }
}