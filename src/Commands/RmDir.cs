namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class RmDir : ICommands
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
            var cf = tfs.CurrentFolder;
            string folder = "";

            foreach(var s in args)
            {
                if (s.StartsWith("-")) continue;
                if (folder == "") folder = s;
                break;
            }

            string[] p = folder.Split("/");
            string name = p.Last();
            if (!cf.ContainsKey(folder)) {
                terminal.Error(this, folder, "no such file or directory");
                return;
            }

            tfs.Retreive(terminal, string.Join("/", p[0..^1]));
            if (cf.Count > 0)
            {
                terminal.Error(this, folder, "folder is not empty");
                return;
            }

            tfs.Path.Pop();
            tfs.CurrentFolder = (tfs.Path.Last().Value as Dictionary<string, Content>)!;
            tfs.PathStr = tfs.Path.ToString() ?? "TH_UNKNOWN_ERROR";
        }
    }
}