namespace AdventOfCode2022;

public class Day06
{
    public int Part1(string input)
    {
        var lengthCheck = 4;
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
        new Day06().Part1(input).Should().Be(value);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day06().Part1(Helper.ReadDay(6)).Should().Be(1647);
    }
    
}