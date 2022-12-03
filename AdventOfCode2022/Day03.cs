namespace AdventOfCode2022;

public class Day03Part1
{
    public int Prioritize(string input)
    {
        return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(GetItemFromBothCompartments)
            .Select(ValueItem)
            .Sum();
    }

    public char GetItemFromBothCompartments(string rucksack)
    {
        return rucksack.Substring(0, rucksack.Length / 2)
            .Intersect(rucksack.Substring(rucksack.Length / 2))
            .First();
    }

    public int ValueItem(char item)
    {
        return item switch
        {
            >= 'a' and <= 'z' => item - 'a' + 1,
            >= 'A' and <= 'Z' => item - 'A' + 27,
            _ => 0
        };
    }
}

public class Day03Part2 : Day03Part1
{
    public int Prioritize3(string input)
    {
        return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Chunk(3)
            .Select(x => x[0].Intersect(x[1]).Intersect(x[2]).First())
            .Sum(ValueItem);
    }
}

[TestFixture]
public class Day03Tests
{
    private string _example = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw";
    
    [Test]
    public void Part1Example()
    {
        var result = new Day03Part1().Prioritize(_example);

        result.Should().Be(157);
    }

    [TestCase('a', 1)]
    [TestCase('b', 2)]
    [TestCase('z', 26)]
    [TestCase('A', 27)]
    [TestCase('B', 28)]
    [TestCase('Z', 52)]
    public void ValueItemTest(char item, int value)
    {
        new Day03Part1().ValueItem(item).Should().Be(value);
    }

    
    [TestCase("vJrwpWtwJgWrhcsFMMfFFhFp", 'p')]
    [TestCase("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 'L')]
    [TestCase("PmmdzqPrVvPwwTWBwg", 'P')]
    [TestCase("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", 'v')]
    [TestCase("ttgJtRGJQctTZtZT", 't')]
    [TestCase("CrZsJsPPZsGzwwsLwLmpwMDw", 's')]
    public void GetItemFromBothCompartmentsTest(string rucksack, char item)
    {
        new Day03Part1().GetItemFromBothCompartments(rucksack).Should().Be(item);
    }

    [Test]
    public void Part1Input()
    {
        var result = new Day03Part1().Prioritize(Helper.ReadDay(3));

        result.Should().Be(8252);
    }

    [Test]
    public void Part2Example()
    {
        var result = new Day03Part2().Prioritize3(_example);

        result.Should().Be(70);
    }

    [Test]
    public void Part2Input()
    {
        var result = new Day03Part2().Prioritize3(Helper.ReadDay(3));

        result.Should().Be(2828);
    }
}