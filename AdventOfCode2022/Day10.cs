using System.Text;

namespace AdventOfCode2022;

public class Day10
{
    public int Part1(string input, Func<int,int> multiplier, params int[] cycles)
    {
        var registerValues = new Dictionary<int, int> { [1] = 1 };
        var last = 1;
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(" ")))
        {
            switch (line[0])
            {
                case "noop":
                    registerValues[last + 1] = registerValues[last];
                    last++;
                    break;
                case "addx":
                    registerValues[last + 1] = registerValues[last];
                    registerValues[last + 2] = registerValues[last] + int.Parse(line[1]);
                    last += 2;
                    break;
            }
        }

        return cycles.Select(x => GetValueAtCycle(x, registerValues) * multiplier(x)).Sum();
    }

    private int GetValueAtCycle(int c, Dictionary<int, int> registerValues)
    {
        return registerValues[c];
    }
    
    public void Part2(string input)
    {
        var registerValues = new Dictionary<int, int> { [1] = 1 };
        var last = 1;
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(" ")))
        {
            switch (line[0])
            {
                case "noop":
                    registerValues[last + 1] = registerValues[last];
                    last++;
                    break;
                case "addx":
                    registerValues[last + 1] = registerValues[last];
                    registerValues[last + 2] = registerValues[last] + int.Parse(line[1]);
                    last += 2;
                    break;
            }
        }

        StringBuilder sb = new StringBuilder();
        for (var cycle = 1; cycle <= 240; cycle++)
        {
            var position = (cycle - 1) % 40;
            var spriteMiddle = registerValues[cycle];
            if (spriteMiddle - 1 <= position && spriteMiddle + 1 >= position)
                sb.Append("■");
            else
                sb.Append(" ");
        }

        for (var i = 0; i < sb.Length; i++)
        {
            if (i % 40 == 0)
                Console.WriteLine();
            Console.Write(sb[i]);
        }
    }
}

[TestFixture]
public class Day10Tests
{
    [Test]
    public void Part2Example()
    {
        var day = new Day10();
        day.Part2(_example2);
    }

    [Test]
    public void Part2Input()
    {
        new Day10().Part2(Helper.ReadDay(10));
    }
    
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