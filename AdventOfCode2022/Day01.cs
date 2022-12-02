using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace AdventOfCode2022;

public static class Day01
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

    public static long MaxCaloriesOfThreeLinq(string input)
    {
        var caleriesByElf = new List<long>();

        var totalCaleriesByElf = 0L;

        foreach (var cal in input.Split(Environment.NewLine))
        {
            if (cal != String.Empty)
            {
                totalCaleriesByElf += long.Parse(cal);
            }
            else
            {
                caleriesByElf.Add(totalCaleriesByElf);
                totalCaleriesByElf = 0L;
                continue;
            }
        }

        return caleriesByElf.OrderByDescending(x => x).Take(3).Sum();
    }
    
    public static long MaxCaloriesOfThree(string input)
    {
        var lines = input.Split(Environment.NewLine);

        var max = new long[3];
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

            for (var j = 2; j >= 0; j--)
            {
                if (max[j] < current)
                {
                    for (var k = 0; k < j; k++)
                    {
                        max[k] = max[k + 1];
                    }

                    max[j] = current;
                    break;
                }
            }

            current = 0;
        }

        return max.Sum();
    }
}

[TestFixture]
public class Day1Benchmark
{
    [Test]
    public void Run()
    {
        var opt = DefaultConfig.Instance
            .AddDiagnoser(MemoryDiagnoser.Default)
            .WithOption(ConfigOptions.DisableOptimizationsValidator, true);
        var summary = BenchmarkRunner.Run(typeof(Day01Tests), opt);
    }
}

[TestFixture]
[SimpleJob(RuntimeMoniker.Net70)]
public class Day01Tests
{
    private readonly string example = @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000";
    
    [Test]
    public void Part1Example()
    {
        var maxCalories = Day01.MaxCalories(example);

        maxCalories.Should().Be(24000L);
    }

    [Test]
    public void Part1Input()
    {
        var input = File.ReadAllText("Data/Day01.txt");
        
        var maxCalories = Day01.MaxCalories(input);

        maxCalories.Should().Be(69310L);
    }

    [Test]
    public void Part2Example()
    {
        var maxCalories = Day01.MaxCaloriesOfThree(example);

        maxCalories.Should().Be(45000L);
    }
    
    [Test]
    [Benchmark]
    public void Part2Input()
    {
        var input = File.ReadAllText("Data/Day01.txt");
        
        var maxCalories = Day01.MaxCaloriesOfThree(input);

        maxCalories.Should().Be(206104L);
    }
    
    [Test]
    [Benchmark]
    public void Part2InputLinq()
    {
        var input = File.ReadAllText("Data/Day01.txt");
        
        var maxCalories = Day01.MaxCaloriesOfThreeLinq(input);
    
        maxCalories.Should().Be(206104L);
    }

}