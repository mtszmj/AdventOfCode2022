namespace AdventOfCode2022;

public class Day04
{
    public int CountInner(string example)
    {
        return example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(',').SelectMany(parts => parts.Split('-'))
                    .Select(int.Parse).ToArray();
                var (l1,l2,r1,r2) = (parts[0], parts[1], parts[2], parts[3]);
                if (l1 <= r1 && l2 >= r2
                    || r1 <= l1 && r2 >= l2)
                    return 1;
                
                return 0;
            }).Sum();
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
        var innerCount = new Day04().CountInner(_example);
        innerCount.Should().Be(2);
    }
    
    [Test]
    public void Part1Input()
    {
        var innerCount = new Day04().CountInner(Helper.ReadDay(4));
        innerCount.Should().Be(542);
    }
    
    public void Part2Example()
    {
        Assert.Fail();
    }
    
    public void Part2Input()
    {
        Assert.Fail();
    }
}