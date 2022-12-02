namespace AdventOfCode2022;

public class Day02Part1
{
    public int Calculate(string input)
    {
        return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (enemy: x[0], me: x[2]))
            .Select(x => SelectionResult(x) + MatchResult(x))
            .Sum();
    }

    public virtual int SelectionResult((char enemy, char me) rps)
    {
        return rps.me switch
        {
            'X' => 1,
            'Y' => 2,
            'Z' => 3,
            _ => throw new InvalidOperationException($"{rps.enemy}")
        };
    }
    
    public virtual int MatchResult((char enemy, char me) rps)
    {
        return rps switch
        {
            ('A', 'X') => 3,
            ('A', 'Y') => 6,
            ('A', 'Z') => 0,
            ('B', 'X') => 0,
            ('B', 'Y') => 3,
            ('B', 'Z') => 6,
            ('C', 'X') => 6,
            ('C', 'Y') => 0,
            ('C', 'Z') => 3,
            _ => throw new InvalidOperationException($"{rps}")
        };
    }
}

public class Day02Part2 : Day02Part1
{
    public override int MatchResult((char enemy, char me) rps)
    {
        return rps.me switch
        {
            'X' => 0,
            'Y' => 3,
            'Z' => 6,
            _ => throw new InvalidOperationException($"{rps.me}")
        };
    }

    public override int SelectionResult((char enemy, char me) rps)
    {
        return rps switch
        {
            ('A', 'X') => 3,
            ('A', 'Y') => 1,
            ('A', 'Z') => 2,
            ('B', 'X') => 1,
            ('B', 'Y') => 2,
            ('B', 'Z') => 3,
            ('C', 'X') => 2,
            ('C', 'Y') => 3,
            ('C', 'Z') => 1,
            _ => throw new InvalidOperationException($"{rps.enemy}")
        };
    }
}

[TestFixture]
public class Day02Tests
{
    private readonly string _example = @"A Y
B X
C Z";

    [Test]
    public void Part1Example()
    {
        var result = new Day02Part1().Calculate(_example);

        result.Should().Be(15);
    }
    
    [Test]
    public void Part1Input()
    {
        var input = File.ReadAllText("Data/Day02.txt");

        var result =  new Day02Part1().Calculate(input);

        result.Should().Be(12586);
    }

    [Test]
    public void Part2Example()
    {
        var result = new Day02Part2().Calculate(_example);

        result.Should().Be(12);
    }
    
    [Test]
    public void Part2Input()
    {
        var input = File.ReadAllText("Data/Day02.txt");

        var result =  new Day02Part2().Calculate(input);

        result.Should().Be(13193);
    }
}