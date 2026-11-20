/* TODO: 
 * - Set this 'CurrentDirectory' variable to enter the home directory when the OS File Tree is complete
*/

using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace TerminHack;

using System.Collections.Generic;

using Folder = Dictionary<string, FileSystem.Content>;
using NodeInfo = KeyValuePair<string, FileSystem.Content>;
using FolderInfo = KeyValuePair<string, Dictionary<string, FileSystem.Content>>;
using File = string;


public class Drive : Folder {}

public partial class FileSystem
{
    private string activeDrive;

    public Folder ActiveDrive, CurrentDrive, CurrentFolder;
    public Stack<FolderInfo> Path;
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
        var options = new JsonSerializerOptions { WriteIndented = true };
        // System.Console.WriteLine(file);
        this.Path = new();

        this.ActiveDrive = JsonSerializer.Deserialize<Drive>(System.IO.File.ReadAllText(this.activeDrive)) ?? new() 
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

        this.CurrentDrive = this.ActiveDrive!;
        this.CurrentFolder = this.CurrentDrive;
        this.Path.Push(new("root", this.CurrentFolder));
        this.UpdatePath();
    }

    public enum NodeType
    {
        UNKNOWN,
        FILE,
        FOLDER
    }

    public union Content(File, Folder)
    {
        public bool IsFile() => this is File;
        public bool IsFolder() => this is Folder;

        public Folder? GetFolder() => this switch
        {
            Folder folder => folder,
            File file => null,
            _ => null
        };

        public string? GetFile() => this switch
        {
            File file => file,
            Folder folder => null,
            _ => null
        };

        public Type? GetType() => this switch
        {
            File file => file.GetType(),
            Folder folder => folder.GetType(),
            _ => null
        };
    }

    public bool HasName(string name)
        => this.CurrentFolder.ContainsKey(name);

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

    public Content? Retreive(Terminal terminal, string path, FileSystem? fs = null)
    {
        fs ??= this;
        if (path.StartsWith("/"))
            fs.EnterDirectory(terminal, path, fs);

        var cf = fs.CurrentFolder;

        switch (path) 
        {
            case "": return cf;
            case "/": 
                fs.EnterDirectory(terminal, path, fs);
                return fs.CurrentFolder;
        }


        var traversal = path.Split("/");
        for (int i = 0; i < traversal.Length; i++)
        {
            string name = traversal[i];
            switch (name)
            {
                case "": 
                case ".":
                case "..":
                    fs.EnterDirectory(terminal, name, fs);
                    if (i >= traversal.Length - 1) 
                        return fs.CurrentFolder;

                    continue;

                default:
                    if (!cf.ContainsKey(name))
                        return null;

                    var item = cf[name];
                    if (i < traversal.Length - 1)
                    {
                        if (item.IsFolder())
                        {
                            fs.EnterDirectory(terminal, name, fs);
                            return fs.Retreive(terminal, string.Join("/", traversal[1..]));
                        }
                        else if (item.IsFile())
                        {
                            return null;
                        }
                        else
                        {
                            System.Console.WriteLine("something went horribly wrong...");
                            System.Console.WriteLine("at FileSystem.Traverse():");
                            System.Console.WriteLine($"  name: {name}, cf.Count: {cf.Count}, item: {item.GetType().FullName}");
                        }
                    }
                    else
                    {
                        if (item.IsFolder())
                        {
                            var f = cf[name].GetFolder();
                            if (f == null)
                            {
                                System.Console.WriteLine("something went horribly wrong...");
                                System.Console.WriteLine("at FileSystem.Traverse():");
                                System.Console.WriteLine($"  name: {name}, cf.Count: {cf.Count}, getf: {f}\ncf[name]: {cf[name].GetType().FullName} = {cf[name]}");
                                return null;
                            }

                            return f;
                        }
                        else if (item.IsFile())
                        {
                            return item;
                        }
                    }
                    break;
            }
        }
        return null;
    }

    public void EnterDirectory(Terminal terminal, string folder, FileSystem? fs = null)
    {
        fs ??= terminal.fs;
        var cf = fs.CurrentFolder;

        if (!cf.ContainsKey(folder))
        {
            switch (folder)
            {
                case null:
                case "":
                case ".": 
                    return;
                case "..":
                    if (this.Path.Count > 0)
                    {
                        this.Path.Pop();
                        this.CurrentFolder = this.Path.Count > 0 ? this.Path.Last().Value : this.CurrentDrive;
                    }
                    return;
                case "/":
                    fs.CurrentFolder = fs.CurrentDrive;
                    fs.Path.Clear();
                    return;
                default:
                    System.Console.WriteLine("no such file or directory");
                    return;
            }
        }

        var item = cf[folder];

        if (!item.IsFolder())
        {
            System.Console.WriteLine("FileSystem.EnterDirectory: {folder} is not a folder");
            return;
        }

        var f = item.GetFolder();
        if (f == null)
        {
            System.Console.WriteLine("something went horribly wrong...");
            System.Console.WriteLine("at FileSystem.EnterDirectory():");
            System.Console.WriteLine($"  folder = string ({folder}), f: {f.GetType().FullName} ({f.ToString}), ");
            return;
        }

        fs.CurrentFolder = f;
        fs.Path.Push(new(folder, f));
    }

    public void WriteSave()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        System.IO.File.WriteAllText(this.activeDrive, JsonSerializer.Serialize<Folder>(this.CurrentDrive, options));
    }

    public void UpdatePath()
    {
        if (this.Path.Count < 1)
        {
            this.PathStr = "/";
            return;
        }

        StringBuilder ss = new();
        foreach (var p in this.Path.Reverse())
        {
            if (p.Key == "root") continue;
            ss.Append($"/{p.Key}");
        }

        this.Path.Reverse();
    
        this.PathStr = ss.ToString();
    }
}