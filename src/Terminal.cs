using System.ComponentModel;
using System.Text;

namespace TerminHack;

public enum Privilage
{
    User,
    SuperUser,
    Root,
}

public record Terminal(string Username, string MachineName)
{
    public FileSystem fs = new(Path.GetFullPath("dir.indented.json"));
    public Privilage PermissionLevel = Privilage.User;
    public History History = new();
    public string Prompt = $"{Username}@{MachineName} $";

    public CommandHandler cmdHnd = new();


    public async void Start()
    {
        while (true)
        {
            this.fs.Path.Clear();
            this.fs.UpdatePath();
            this.Prompt = $"{Username}@{MachineName}: {this.fs.PathStr ?? "/"}# ";
            // this.Prompt = $"{Username}@{MachineName}: {this.fs.Path_ToString()}{PermissionLevel switch
            // {
            //     Privilage.User => '$',
            //     Privilage.SuperUser => '#',
            //     Privilage.Root => '#'
            // }} ";

            System.Console.Write(this.Prompt);

            string input = Console.ReadLine() ?? "";
            if (input == "") continue;
            if (input.StartsWith("exit")) break;


            string[] split = input.Split(' ');
            List<string> args = new();
            string str = "";

            bool concat = false, connected = false;
            foreach (var s in split)
            {
                if (s.EndsWith("\\"))
                {
                    connected = true;
                    str += $"{s[0..^1]} ";
                    continue;
                }

                if (connected)
                {
                    connected = false;
                    str += s;
                    args.Add(str);
                    continue;
                }

                if (concat)
                {
                    if (s.StartsWith('"'))
                    {
                        this.Print("INTERNAL ERROR: QUOTE ESCAPE MAY HAVE BEEN ATTEMPTED - ABORTING WITHOUT SAVING");
                        Environment.Exit(1);
                    }
                    if (s.EndsWith('"'))
                    {
                        str += $" {s[0..^1]}";
                        concat = false;
                        args.Add(str);
                        str = "";
                        continue;
                    }
                    str += $" {s}";
                    continue;
                }

                if (s.StartsWith('"'))
                {
                    str += s[1..^0];
                    concat = true;
                    continue;
                }

                if (s.StartsWith("-")) continue;
                // if (str == "") str = s;
                args.Add(s);
            }

            if (args.Count == 1 && args[0] == "")
                args = new();

            cmdHnd.Submit(this, args.ToArray().AsSpan());
        }
    }

    public void Print(string? msg = "") => Console.WriteLine(msg);
    public void Error(object? cmd, ReadOnlySpan<char> arg, ReadOnlySpan<char> msg)
        => Console.WriteLine(arg == null
            ? $"{(cmd != null ? cmd.GetType().Name.ToLower() : "bash")}: {msg}"
            : $"{(cmd != null ? cmd.GetType().Name.ToLower() : "bash")}: {arg}: {msg}");
}