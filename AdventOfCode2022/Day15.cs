using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day15
{
    public Sensor ParseLine(string line)
    {
        var match = Regex.Match(line, @"^Sensor at x=(?<x>[-\d]+), y=(?<y>[-\d]+): closest beacon is at x=(?<xb>[-\d]+), y=(?<yb>[-\d]+)$", RegexOptions.Compiled);
        return new Sensor(int.Parse(match.Groups["x"].Value), 
            int.Parse(match.Groups["y"].Value),
            int.Parse(match.Groups["xb"].Value), 
            int.Parse(match.Groups["yb"].Value)
            );
    }

    public int Part1(string example, int row)
    {
        var sensors = example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseLine);
        
        var coveredRange = sensors
            .Select(x => x.RangeAtRow(row))
            .Aggregate(new HashSet<int>(),
                (hs, x) =>
                {
                    hs.UnionWith(x);
                    return hs;
                })
            ;

        var beaconsAtRow = sensors.Where(x => x.BeaconY == row).Select(x => x.BeaconX).ToHashSet();

        return coveredRange.Except(beaconsAtRow).Count();
    }
}

public class Sensor
{
    public Sensor(int x, int y, int beaconX, int beaconY)
    {
        X = x;
        Y = y;
        BeaconX = beaconX;
        BeaconY = beaconY;
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    public int BeaconX { get; set; }
    public int BeaconY { get; set; }

    public int BeaconDistance() => Math.Abs(BeaconX - X) + Math.Abs(BeaconY - Y);

    public IEnumerable<int> RangeAtRow(int row)
    {
        var distance = Math.Abs(row - Y);
        var signalStrength = BeaconDistance();
        if (distance > signalStrength)
            return Enumerable.Empty<int>();

        var diff = signalStrength - distance;
        return Enumerable.Range(X - diff, 2 * diff + 1);
    }
}

[TestFixture]
public class Day15Tests
{
    public const string Example = @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";

    [Test]
    public void ParseTest()
    {
        var sensor = new Day15().ParseLine("Sensor at x=2, y=18: closest beacon is at x=-2, y=15");
        sensor.X.Should().Be(2);
        sensor.Y.Should().Be(18);
        sensor.BeaconX.Should().Be(-2);
        sensor.BeaconY.Should().Be(15);
    }

    [TestCase(2, 18, -2, 15, 7)]
    public void BeaconDistanceTest(int x, int y, int xb, int yb, int expected)
    {
        new Sensor(x, y, xb, yb).BeaconDistance().Should().Be(expected);
    }
    
    [TestCase(8, 7, 2, 10, 7, -1, 17)]
    [TestCase(8, 7, 2, 10, 6, 0, 16)]
    [TestCase(8, 7, 2, 10, 8, 0, 16)]
    [TestCase(8, 7, 2, 10, -2, 8, 8)]
    [TestCase(8, 7, 2, 10, 16, 8, 8)]
    public void BeaconRangeAtRowTest(int x, int y, int xb, int yb, int row, int xrange1, int xrange2)
    {
        var result = new Sensor(x, y, xb, yb).RangeAtRow(row).ToList();
        result.Should().BeEquivalentTo(Enumerable.Range(xrange1, xrange2 - xrange1 + 1));
    }
    
    [TestCase(8, 7, 2, 10, -3)]
    [TestCase(8, 7, 2, 10, -17)]
    public void EmptyBeaconRangeAtRowTest(int x, int y, int xb, int yb, int row)
    {
        var result = new Sensor(x, y, xb, yb).RangeAtRow(row).ToList();
        result.Should().BeEmpty();
    }

    [Test]
    public void Part1Example()
    {
        new Day15().Part1(Example, 10).Should().Be(26);
    }

    [Test]
    public void Part1Input()
    {
        new Day15().Part1(Helper.ReadDay(15), 2000000).Should().Be(5716881);
    }
}