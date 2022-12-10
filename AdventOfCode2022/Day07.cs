using System.Text;

namespace AdventOfCode2022;

public class Day07
{
    public long SolvePart1(string input)
    {
        var commands = ParseCommands(input);
        var fs = new FileSystem();
        foreach (var command in commands) fs.ApplyCommand(command);

        fs.Print();

        var sum = fs.SumDirsBelowTotalSize(100000);
        return sum;
    }
    
    public long SolvePart2(string input)
    {
        var commands = ParseCommands(input);
        var fs = new FileSystem();
        foreach (var command in commands) fs.ApplyCommand(command);

        var min = fs.FindMinAboveValue(70000000, 30000000);
        return min;
    }

    public List<ICommand> ParseCommands(string input)
    {
        var commands = new List<ICommand>();
        var commandData = input.Split("$ ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var cmdData in commandData)
        {
            var split = cmdData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var line = split[0];

            var cmd = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (cmd.Length == 2)
            {
                commands.Add((cmd[0], cmd[1]) switch
                {
                    ("cd", "/") => new ChangeDirRoot(),
                    ("cd", "..") => new ChangeDirOut(),
                    ("cd", _) => new ChangeDirIn(cmd[1])
                });
                continue;
            }

            var listInfo = new ListInfo();
            commands.Add(listInfo);
            for (var index = 1; index < split.Length; index++)
            {
                var data = split[index];
                var info = data.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (info[0] == "dir")
                    listInfo.AddDirectory(new SubDirInfo(info[1]));
                else
                    listInfo.AddFile(new FileInfo(info[1], long.Parse(info[0])));
            }
        }

        return commands;
    }
}

public class FileSystem
{
    private Directory _current;
    private readonly Directory _root = new();

    public FileSystem()
    {
        _current = _root;
    }

    public void ApplyCommand(ICommand command)
    {
        command.Apply(this);
    }

    public void CdIn(string name)
    {
        _current = _current.Children[name];
    }

    public void CdOut()
    {
        _current = _current.Parent ?? _root;
    }

    public void CdRoot()
    {
        _current = _root;
    }

    public void AddDir(string name)
    {
        _current.AddChild(name);
    }

    public void AddFile(FileInfo fi)
    {
        _current.AddFile(fi);
    }

    public void Print()
    {
        var sb = new StringBuilder();
        _root.Print(sb, 0);
        Console.WriteLine(sb.ToString());
    }

    public long SumDirsBelowTotalSize(long totalSize)
    {
        var result = 0L;
        var dirs = GetAllDirs();
        foreach (var dir in dirs)
        {
            var ts = dir.TotalSize();
            if (ts < totalSize)
            {
                result += ts;
            }
        }

        return result;
    }
    
    public long FindMinAboveValue(long totalSize, long required)
    {
        var occupied = _root.TotalSize();
        var free = totalSize - occupied;
        var searched = required - free;
        
        var min = totalSize;
        var dirs = GetAllDirs();
        foreach (var dir in dirs)
        {
            var ts = dir.TotalSize();
            if (ts < min && ts > searched)
            {
                min = ts;
            }
        }

        return min;
    }

    public IEnumerable<Directory> GetAllDirs()
    {
        yield return _root;
        foreach (var allChild in _root.GetAllChildren())
        {
            yield return allChild;
        }
    }
}

public class Directory
{
    public HashSet<FileInfo> Files = new();

    public Directory()
    {
        Name = "Root";
    }

    public Directory(string name, Directory parent)
    {
        Name = name;
        Parent = parent;
    }

    public static Directory CreateRoot => new();

    public string Name { get; }
    public Directory? Parent { get; }
    public Dictionary<string, Directory> Children { get; } = new();

    public bool IsRoot => Parent is null;

    public static Directory CreateChild(string name, Directory parent)
    {
        return new(name, parent);
    }

    public void AddFile(FileInfo fi)
    {
        Files.Add(fi);
    }

    public void AddChild(string name)
    {
        if (!Children.ContainsKey(name)) Children.Add(name, CreateChild(name, this));
    }

    public long TotalSize()
    {
        var ts = 0L;
        foreach (var file in Files)
        {
            ts += file.Size;
        }

        foreach (var dir in Children)
        {
            ts += dir.Value.TotalSize();
        }

        return ts;
    }
    
    public void Print(StringBuilder sb, int ident)
    {
        for (var i = 0; i < ident; i++)
            sb.Append("  ");
        sb.AppendLine($"- {Name} (dir) (Total: {TotalSize()})");

        foreach (var subDir in Children.Values) subDir.Print(sb, ident + 1);

        foreach (var file in Files) file.Print(sb, ident + 1);
    }

    public IEnumerable<Directory> GetAllChildren()
    {
        foreach (var subDir in Children.Values)
        {
            yield return subDir;
            foreach (var subSubDir in subDir.GetAllChildren())
            {
                yield return subSubDir;
            }
        }
    }
}

public interface ICommand
{
    void Apply(FileSystem fs);
}

public class ChangeDirIn : ICommand
{
    public ChangeDirIn(string dir)
    {
        Dir = dir;
    }

    public string Dir { get; }

    public void Apply(FileSystem fs)
    {
        fs.CdIn(Dir);
    }
}

public class ChangeDirOut : ICommand
{
    public void Apply(FileSystem fs)
    {
        fs.CdOut();
    }
}

public class ChangeDirRoot : ICommand
{
    public void Apply(FileSystem fs)
    {
        fs.CdRoot();
    }
}

public class ListInfo : ICommand
{
    private readonly List<SubDirInfo> _directories = new();
    private readonly List<FileInfo> _files = new();

    public void Apply(FileSystem fs)
    {
        foreach (var dir in _directories) fs.AddDir(dir.DirInfo);

        foreach (var file in _files) fs.AddFile(file);
    }

    public void AddFile(FileInfo fi)
    {
        _files.Add(fi);
    }

    public void AddDirectory(SubDirInfo di)
    {
        _directories.Add(di);
    }
}

public record FileInfo(string FileName, long Size)
{
    public void Print(StringBuilder sb, int ident)
    {
        for (var i = 0; i < ident; i++)
            sb.Append("  ");

        sb.AppendLine($"- {FileName} (file, size={Size})");
    }
}

public record SubDirInfo(string DirInfo);

[TestFixture]
public class Day07Tests
{
    private readonly string _example = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

    [Test]
    public void Part1Example()
    {
        new Day07().SolvePart1(_example).Should().Be(95437L);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day07().SolvePart1(Helper.ReadDay(7)).Should().Be(1182909L);
    }
    
    [Test]
    public void Part2Example()
    {
        new Day07().SolvePart2(_example).Should().Be(24933642);
    }
    
    [Test]
    public void Part2Input()
    {
        new Day07().SolvePart2(Helper.ReadDay(7)).Should().Be(2832508L);
    }
}