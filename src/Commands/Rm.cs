namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    // use "rmdir" for empty folders when running with recursion flag "-r"
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
            string folder = "";

            foreach(var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (folder == "") folder = s;
                break;
            }

            string[] ss = folder.Split("/");
            if (ss.Length < 1 ) {
                terminal.Print(@"rm: missing operand
Try 'rm --help' for more information");
                return;
            }
            
            Content? p = null;
            if (ss.Length > 1)
                p = tfs.Retreive(terminal, string.Join("/", ss[0..^1]));

            if (!p.HasValue) return;
            Content parent = p.Value;
            if (parent.Value is string)
            {
                terminal.Error(this, folder, "parent directory is a file");
                return;
            }

            Dictionary<string, Content> f = parent.Value as Dictionary<string, Content> ?? tfs.CurrentFolder;
            string n = ss[ss.Length-1];
            if (f.ContainsKey(n))
            {
                f.Remove(n);
            }

            tfs.WriteSave();
        }
    }
}