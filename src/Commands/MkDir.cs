namespace TerminHack;

// using Newtonsoft.Json.Linq;
using static FileSystem;

public partial class Command
{
    public class MkDir : ICommands
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
                terminal.Print(@"Error: the following required arguments were not provided:
    <dirs>...

Usage: mkdir [OPTION]... DIRECTORY...

For more information, try '--help'");
                return;
            }
            
            Content? parent = tfs.CurrentFolder;
            if (sstr.Length > 1)
                parent = tfs.Retreive(terminal, sstr.Length > 2 ? string.Join("/",  sstr[0..^1]) : sstr[0], new(tfs));

            switch (parent)
            {
                case string file:
                    terminal.Error(this, name, "File exists");
                    return;

                case Dictionary<string, Content> folder:
                    folder.Add(sstr.Length > 1 ? sstr[sstr.Length-1] : sstr[0], new Content(new Dictionary<string, Content>()));
                    tfs.WriteSave();
                    return;

                case null:
                    terminal.Print("mkdir: No such file or directory");
                    return;
            }
        }
    }
}