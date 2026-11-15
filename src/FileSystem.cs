using System.Linq;
using System.Text.Json;


namespace TerminHack;

using System.Collections.Generic;

using Folder = Dictionary<string, FileSystem.Content>;
using NodeInfo = KeyValuePair<string, FileSystem.Content>;
using File = string;
using System.Text;
using System.Xml;

// Reference Types - not to be used as nor confused with actual types.
// To be used as an Explicit Specifiers 
// That's "Dictionary<string, string>" and "Dictionary<string, FileSystem.Node[]>"

public partial class FileSystem
{

    private string activeDrive;

    public Folder ActiveDrive, CurrentDrive, CurrentFolder;
    public Stack<NodeInfo> Path;
    public string PathStr;

    public FileSystem(FileSystem fs)
    {
        this.activeDrive = fs.activeDrive;
        this.ActiveDrive = fs.ActiveDrive;
        this.CurrentDrive = fs.CurrentDrive;
        this.CurrentFolder = fs.CurrentFolder;
        this.Path = fs.Path;
        this.PathStr = fs.PathStr;
    }

    public FileSystem(string file)
    {
        this.activeDrive = file;
        // var options = new JsonSerializerOptions { WriteIndented = true };
        // System.Console.WriteLine(file);
        this.Path = new();
        try
        {
            this.ActiveDrive = JsonSerializer.Deserialize<Folder>(System.IO.File.ReadAllText(this.activeDrive));
        }
        catch
        {
            // System.Console.WriteLine(JsonConvert.SerializeObject(this.ActiveDrive, Formatting.Indented));
            this.ActiveDrive = new()
            {
                {
                    "TH_INTERNAL_FS_ERROR", 
                    new Content(new Dictionary<string, Content>() 
                    {
                        {
                            "fs_load_failed.log",
                            new Content("either the file or contents are missing or corrupted or the code is bugged")
                        }
                    })
                }
            };
            System.Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(this.ActiveDrive));
        }
        finally
        {
            this.CurrentDrive = this.ActiveDrive!;
            this.CurrentFolder = this.CurrentDrive;
            this.Path.Push(new("root", this.CurrentFolder));
            this.PathToStr();
        }
    }

    public enum NodeType
    {
        UNKNOWN,
        FILE,
        FOLDER
    }

    public union Content(string, Folder)
    {
        public bool IsFile()   => this is string;
        public bool IsFolder() => this is Folder;

        public Folder? GetFolder() => this switch {
            Folder folder => folder,
            string file => null,
            _ => null
        };

        public string? GetFile() => this switch {
            string file => file,
            Folder folder => null,
            _ => null
        };
    }

    public bool HasName(string name)
        => this.CurrentFolder.ContainsKey(name);

/*
    public Folder? GetFolderByName(Folder src, string name = "")
    {
        if(string.IsNullOrEmpty(name)
        || string.IsNullOrWhiteSpace(name)) return null;

        if (src == null) src = this.CurrentFolder;
        
        object? item = src.Contents.SingleOrDefault(x => x.Name == name);
        Folder? result = item as Folder;
        if (item == null) {
            Console.WriteLine($"cannot access '{name}': No such file or directory");
            return null;
        }
        if (item is JObject) result = JsonConvert.DeserializeObject<Folder>(item.ToString()!);
        if (result == null) {
            Console.WriteLine($"{name} is not a directory");
            return null;
        }

        return result;
    }
*/



    public NodeInfo? GetFile(Folder? dir, string name)
    {
        if (dir.ContainsKey(name)) 
        {
            var entry = dir[name];
            return entry.IsFile()
                ? new(name, entry)
                : null;
        }
        return null;
    }

    public void TraverseUp()
    {
        if (this.Path.Count > 1)
        {
            this.CurrentFolder = (this.Path.Last().Value.Value as Folder);
            this.Path.Pop();
            // List<Folder> pwd = this.PathStr.Split("/")[0..-2];
            this.PathStr = $"{string.Join("/", this.Path.Reverse().ToArray())}";
        }
    }

    public Folder? Traverse(Terminal terminal, string path)
    {
        var tfs = terminal.fs;
        if (path == "" || path == ".") return tfs.CurrentFolder!; // TODO: Set this line to enter the home directory when the OS File Tree is complete
        if (path == "..")
        {
            TraverseUp();
            return this.CurrentFolder!;
        }

        object? res = tfs.Retreive(terminal, path);
        Folder? f = res as Folder;
        if (f == null) return f;

        this.Path.Push(new(path.Split("/").Last(),f!));
        this.CurrentFolder = f!;
        string pstr = string.Join("/", tfs.Path.Reverse());

        if (pstr != "/")
            tfs.PathStr = pstr[Convert.ToInt32(pstr.StartsWith("//"))..^(Convert.ToInt32(pstr.EndsWith("/")))];
            
        /* string[] t = path.Split("/"), sdir = t[1..^0];
        string folder = t[0];

        object? res = tfs.Retreive(terminal, folder);
        if (res == null)
        {
            terminal.Error(null, path, "No such file or directory");
            Folder? f = null;
        }

        if (res is Folder)
        {
            Folder? f = (res as Folder);
            if (f == null)
            {
                f = null;
                terminal.Error(null, path, "No such file or directory");
                return f;
            }

            
            this.Path.Push(f!);
            this.CurrentFolder = f!;
            string pstr = string.Join("/", tfs.Path.Reverse());

            if (pstr != "/")
                tfs.PathStr = pstr[Convert.ToInt32(pstr.StartsWith("//"))..^(Convert.ToInt32(pstr.EndsWith("/")))];

            if (sdir.Length > 0 && folder.Length > 0)
            {
                res = tfs.Traverse(terminal, string.Join("/", sdir));
                tfs.TraverseUp();
                return (Folder?)res;
            }

            tfs.TraverseUp();
            return f;
        }
        else if (res is File)
        {
            File? f = res as File;
            if (f == null)
            {
                terminal.Error(null, path, "No such file or directory");
                return null;
            }
            // terminal.Print($"{tfs.PathStr}/{path}");
            // terminal.Print(tfs.PathStr == "/" ? $"/{path}" : $"{tfs.PathStr}/{path}");

            string pwd = $"{tfs.PathStr}/{path}";
            if (pwd.StartsWith("//"))
                terminal.Print(pwd.EndsWith("/") ? pwd[1..^1] : pwd[1..^0]);

           return null;
        } */
        return null;
    }

// 

    public Content? Retreive(Terminal terminal, string path)
    {
        var tfs = terminal.fs;
        string[] traversal = path.Split("/");
        Folder? res = null;
        Content? node = null;

        var tP = tfs.Path;
        var tps = tfs.PathStr;
        var tcf = tfs.CurrentFolder;

        var tmp = new FileSystem(this);
        for (int i = 0; i < traversal.Length; i++)
        {
            var t = traversal[i];
            if (tcf.ContainsKey(t))
            {
                // if (i < traversal.Length-1)
                // {
                if (tmp.CurrentFolder.ContainsKey(t))
                {
                    var tmpCF = tmp.CurrentFolder[t];
                    
                    if (traversal.Length > 1) {   
                        if (tmpCF.IsFolder())
                        {
                            tmpCF = new Content(node.Value.Value);
                            if (traversal.Length > 1)
                                tmpCF = tmp.Traverse(terminal, string.Join("/", traversal[1..]));
                            else 
                                if ((tmpCF.Value as Folder).ContainsKey(traversal[0])) {
                                    return tmpCF.Value[traversal[0]];
                                }
                        }
                        else if (tmp.CurrentFolder[t].IsFiles())
                        {
                            return null;
                        }
                    }
                }

                // var trav = tmp.Traverse(terminal, string.Join("/", t));
                // if (trav == null) {

                // }
                //     node = tmp.Traverse(terminal, string.Join("/", t));
                //     if (node)
                    // if (!node.Value.IsFolder())
                        // return null;
                    // continue;
                // }
                // else return node;
            } else return null;
        }

        return node;
    }

    // public Folder? Open(JObject jobj)
    // // {
    //     => JsonConvert.DeserializeObject<Folder>(jobj.ToString());
        // return null;

    // } 
    
    public void WriteSave()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        System.IO.File.WriteAllText(this.activeDrive, JsonSerializer.Serialize<Folder>(this.CurrentDrive, options));
    }

    public void PathToStr()
    {
        StringBuilder ss = new();
        foreach(var p in this.Path)
        {
            ss.Append(p.Key == "root" ? "/" : $"{p.Key}/");
        }
        this.PathStr = ss.ToString();
        // this.PathStr = ss.ToString()[0..(ss.Length-1)];
    }
}