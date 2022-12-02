namespace AdventOfCode2022;

public class Day02
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
            _ => throw new InvalidOperationException()
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
            _ => throw new InvalidOperationException()
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
        var result = new Day02().Calculate(_example);

        result.Should().Be(15);
    }
    
    [Test]
    public void Part1Input()
    {
        var input = File.ReadAllText("Data/Day02.txt");

        var result =  new Day02().Calculate(input);

        result.Should().Be(12586);
    }
}