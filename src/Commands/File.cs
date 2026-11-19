namespace TerminHack;

using static TerminHack.FileSystem;

public partial class Command
{
    public class File : ICommands
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
            foreach(string s in args)
            {
                var r = tfs.Retreive(terminal, s, new(tfs));
                terminal.Print(r switch
                {
                    string => $"{s}: ASCII text",
                    Dictionary<string, Content> => $"{s}: directory",
                    _             => $"INTERNAL ERROR: Unknown file type {s.GetType().FullName}"
                });
            }
        }
    }
}