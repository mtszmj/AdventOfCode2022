namespace AdventOfCode2022;

public class Day10
{
    public int Part1(string input, Func<int,int> multiplier, params int[] cycles)
    {
        var cycle = 1;
        var registerValues = new List<(int, int)> {(1, 1)};
        var lastValue = (cycle: 1, value: 1);
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(" ")))
        {
            lastValue = line[0] switch
            {
                "noop" => lastValue with { cycle = lastValue.cycle + 1 },
                "addx" => (lastValue.cycle + 2, lastValue.value + int.Parse(line[1]))
            };
            registerValues.Add(lastValue);
        }

        return cycles.Select(x => GetValueAtCycle(x) * multiplier(x)).Sum();

        int GetValueAtCycle(int c)
        {
            return registerValues.TakeWhile(x => x.Item1 <= c).Last().Item2;
        }
    }
}

[TestFixture]
public class Day10Tests
{
    [Test]
    public void Part1Example1()
    {
        var day = new Day10();
        day.Part1(_example1, x => 1, 1).Should().Be(1);
        day.Part1(_example1, x => 1, 2).Should().Be(1);
        day.Part1(_example1, x => 1, 3).Should().Be(1);
        day.Part1(_example1, x => 1, 4).Should().Be(4);
        day.Part1(_example1, x => 1, 5).Should().Be(4);
        day.Part1(_example1, x => 1, 6).Should().Be(-1);
    }
    
    [Test]
    public void Part1Example2()
    {
        var day = new Day10();
        day.Part1(_example2, x => 1, 20).Should().Be(21);
        day.Part1(_example2, x => 1, 60).Should().Be(19);
        day.Part1(_example2, x => 1, 100).Should().Be(18);
        day.Part1(_example2, x => 1, 140).Should().Be(21);
        day.Part1(_example2, x => 1, 180).Should().Be(16);
        day.Part1(_example2, x => 1, 220).Should().Be(18);
        
        day.Part1(_example2, x => x, 20).Should().Be(420);
        day.Part1(_example2, x => x, 60).Should().Be(1140);
        day.Part1(_example2, x => x, 100).Should().Be(1800);
        day.Part1(_example2, x => x, 140).Should().Be(2940);
        day.Part1(_example2, x => x, 180).Should().Be(2880);
        day.Part1(_example2, x => x, 220).Should().Be(3960);
        
        day.Part1(_example2, x => x, 20,60,100,140,180,220).Should().Be(13140);
    }

    [Test]
    public void Part1Input()
    {
        new Day10().Part1(Helper.ReadDay(10), x => x, 20,60,100,140,180,220).Should().Be(14040);
    }
    
    private string _example1 = @"noop
addx 3
addx -5";

    private string _example2 = @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop";
}