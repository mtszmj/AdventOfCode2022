namespace AdventOfCode2022;

public class Day01
{
    public static long MaxCalories(string input)
    {
        var lines = input.Split(Environment.NewLine);

        var max = 0L;
        var current = 0L;
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            
            if (!string.IsNullOrEmpty(line))
            {
                current += long.Parse(line);
                if (index < lines.Length - 1)
                {
                    continue;
                }
            }
            
            max = max < current ? current : max;
            current = 0L;
        }

        return max;
    }
}

[TestFixture]
public class Day01Tests
{
    [Test]
    public void Part1Example()
    {
        var input = @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000";

        var maxCalories = Day01.MaxCalories(input);

        maxCalories.Should().Be(24000L);
    }

    [Test]
    public void Part1Input()
    {
        var input = File.ReadAllText("Data/Day01.txt");
        
        var maxCalories = Day01.MaxCalories(input);

        maxCalories.Should().Be(69310L);
    }
}