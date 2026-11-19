namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class Rm : ICommands
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
                {
                    switch(s[1..^0])
                    {
                        case "r":
                            recursive = true;
                            break;
                    }
                    continue;
                }
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
                    terminal.Error(this, name, "File exists");
                    break;

                case Dictionary<string, Content> folder:
                    if (folder[name].IsFolder() && recursive == false)
                    {   
                        terminal.Error(this, $"Cannot remove '{dir}'", "Is a directory");
                        return;
                    }
                    folder.Remove(name);
                    tfs.WriteSave();
                    break;

                case null:
                    terminal.Print("rm: No such file or directory");
                    break;
            }
        }
    }
}