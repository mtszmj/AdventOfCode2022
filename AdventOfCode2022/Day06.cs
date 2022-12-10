namespace AdventOfCode2022;

public class Day06
{
    public int Solve(string input, int lengthCheck)
    {
        var span = input.AsSpan();
        var hs = new HashSet<char>();
        for (var i = 0; i < span.Length - lengthCheck ; i++)
        {
            hs.Clear();
            for (var j = i; j < i+lengthCheck; j++)
            {
                hs.Add(span[j]);
            }

            if (hs.Count == lengthCheck)
                return i + lengthCheck;
        }

        return -1;
    }
}

[TestFixture]
public class Day06Tests
{
    [TestCase("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7)]
    [TestCase("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
    [TestCase("nppdvjthqldpwncqszvftbrmjlhg", 6)]
    [TestCase("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
    [TestCase("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11)]
    public void Part1Example(string input, int value)
    {
        new Day06().Solve(input, 4).Should().Be(value);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day06().Solve(Helper.ReadDay(6), 4).Should().Be(1647);
    }
    
    [TestCase("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19)]
    [TestCase("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
    [TestCase("nppdvjthqldpwncqszvftbrmjlhg", 23)]
    [TestCase("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
    [TestCase("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
    public void Part2Example(string input, int value)
    {
        new Day06().Solve(input, 14).Should().Be(value);
    }
    
    [Test]
    public void Part2Input()
    {
        new Day06().Solve(Helper.ReadDay(6), 14).Should().Be(2447);
    }
    
}