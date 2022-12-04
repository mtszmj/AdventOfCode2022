namespace AdventOfCode2022;

public class Day04
{
    public int Part1(string input)
    {
        return GetValues(input).Count(IsInner);
    }

    public int Part2(string input)
    {
        return GetValues(input).Count(AreOverlapping);
    }

    private bool AreOverlapping((int l1, int l2, int r1, int r2) arg)
    {
        return arg.l1 <= arg.r2 
               && arg.l2 >= arg.r1
               ;
    }

    private bool IsInner((int l1, int l2, int r1, int r2) arg)
    {
        return (arg.l1 <= arg.r1 && arg.l2 >= arg.r2
                || arg.r1 <= arg.l1 && arg.r2 >= arg.l2);
    }

    public IEnumerable<(int l1, int l2, int r1, int r2)> GetValues(string input)
    {
        return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(',').SelectMany(parts => parts.Split('-'))
                    .Select(int.Parse).ToArray();
                return (parts[0], parts[1], parts[2], parts[3]);
            });
    }
}

[TestFixture]
public class Day04Tests
{
    public const string _example = @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8";
    
    [Test]
    public void Part1Example()
    {
        var innerCount = new Day04().Part1(_example);
        innerCount.Should().Be(2);
    }
    
    [Test]
    public void Part1Input()
    {
        var innerCount = new Day04().Part1(Helper.ReadDay(4));
        innerCount.Should().Be(542);
    }
    
    [Test]
    public void Part2Example()
    {
        var innerCount = new Day04().Part2(_example);
        innerCount.Should().Be(4);
    }

    [Test]
    public void Part2Input()
    {
        var innerCount = new Day04().Part2(Helper.ReadDay(4));
        innerCount.Should().Be(900);
    }
}