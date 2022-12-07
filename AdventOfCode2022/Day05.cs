using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day05
{
    public string SolvePart1(string input)
    {
        var inputParts = input.Split($"{Environment.NewLine}{Environment.NewLine}");

        var cargo = ParseCrates<Cargo>(inputParts[0]);
        var commands = ParseCommands(inputParts[1]);

        cargo.ApplyCommands(commands);
        return cargo.GetTops();
    }
    
    public string SolvePart2(string input)
    {
        var inputParts = input.Split($"{Environment.NewLine}{Environment.NewLine}");

        var cargo = ParseCrates<Cargo2>(inputParts[0]);
        var commands = ParseCommands(inputParts[1]);

        cargo.ApplyCommands(commands);
        return cargo.GetTops();
    }

    public Cargo ParseCrates<T>(string inputPart) where T : Cargo, new()
    {
        var lines = inputPart.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var stackIndexes = new List<int>();
        var lastLine = lines[^1];
        for (var i = 0; i < lastLine.Length; i++)
        {
            if (!char.IsDigit(lastLine[i])) continue;
            stackIndexes.Add(i);
            i++;
        }

        var cargo = new T();
        cargo.Init(stackIndexes.Count);

        for (var i = lines.Length - 2; i >= 0; i--)
        {
            for (var stackIndex = 0; stackIndex < stackIndexes.Count; stackIndex++)
            {
                var c = lines[i][stackIndexes[stackIndex]];
                if (char.IsLetter(c))
                {
                    cargo.Add(stackIndex + 1, c);
                }
            }
        }

        return cargo;
    }
    
    public List<Command> ParseCommands(string inputPart)
    {
        var commands = inputPart.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x =>
        {
            var match = Regex.Match(x, @"move (?<count>\d+) from (?<from>\d+) to (?<to>\d+)", RegexOptions.Compiled);
            return new Command(int.Parse(match.Groups["count"].Value),
                int.Parse(match.Groups["from"].Value),
                int.Parse(match.Groups["to"].Value));
        }).ToList();

        return commands;
    }
}

public class Cargo2 : Cargo
{
    public override void ApplyCommand(Command cmd)
    {
        var crateMover = new Stack<char>();

        for (var i = 0; i < cmd.Count; i++)
        {
            crateMover.Push(Take(cmd.From));
        }

        while(crateMover.Any())
        {
            Add(cmd.To, crateMover.Pop());
        }
    }
}

public class Cargo
{
    public void Init(int stacks)
    {
        Stacks = new Stack<char>[stacks];
        for (var i = 0; i < stacks; i++)
            Stacks[i] = new Stack<char>();
    }

    private Stack<char>[] Stacks { get; set; }

    public void Add(int stack, char value) => Stacks[stack-1].Push(value);
    protected char Take(int stack) => Stacks[stack-1].Pop();

    public void ApplyCommands(IEnumerable<Command> cmds)
    {
        foreach (var cmd in cmds)
            ApplyCommand(cmd);
    }
    
    public virtual void ApplyCommand(Command cmd)
    {
        for(var i = 0; i < cmd.Count; i++)
            Add(cmd.To, Take(cmd.From));
    }

    public string GetTopToBottom(int stack) => Stacks[stack - 1].ToList().Aggregate("", (s, c) => $"{s}{c}");

    public string GetTops()
    {
        var sb = new StringBuilder();
        foreach (var stack in Stacks)
            sb.Append(stack.Peek());

        return sb.ToString();
    }
}

public record Command(int Count, int From, int To)
{
    
}

[TestFixture]
public class Day05Tests
{
    private static string _example = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
    
    [Test]
    public void Part1Example()
    {
        var result = new Day05().SolvePart1(_example);

        result.Should().Be("CMZ");
    }

    [Test]
    public void Part1Input()
    {
        var result = new Day05().SolvePart1(Helper.ReadDay(5));

        result.Should().Be("NTWZZWHFV");
    }

    [Test]
    public void ParseCommandsTest()
    {
        var commandInput = @"move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

        var commands = new Day05().ParseCommands(commandInput);

        commands.Should().HaveCount(4);
        commands[0].Should().Be(new Command(1, 2, 1));
        commands[1].Should().Be(new Command(3, 1, 3));
        commands[2].Should().Be(new Command(2, 2, 1));
        commands[3].Should().Be(new Command(1, 1, 2));
    }
    [Test]
    public void ParseCargoTest()
    {
        var commandInput = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 ";

        var cargo = new Day05().ParseCrates<Cargo>(commandInput);

        cargo.GetTopToBottom(1).Should().Be("NZ");
        cargo.GetTopToBottom(2).Should().Be("DCM");
        cargo.GetTopToBottom(3).Should().Be("P");
    }

    [Test]
    public void Part2Example()
    {
        var result = new Day05().SolvePart2(_example);

        result.Should().Be("MCD");       
    }

    [Test]
    public void Part2Input()
    {
        var result = new Day05().SolvePart2(Helper.ReadDay(5));

        result.Should().Be("BRZGFVBTJ");
    }
}