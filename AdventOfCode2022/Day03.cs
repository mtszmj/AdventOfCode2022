namespace AdventOfCode2022;

public class Day03
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
        var left = new HashSet<char>();
        var right = new HashSet<char>();
        for (var index = 0; index < rucksack.Length / 2; index++)
        {
            var l = rucksack[index];
            var r = rucksack[rucksack.Length - 1 - index];
            left.Add(l);
            right.Add(r);
            if (left.Contains(r))
                return r;
            if (right.Contains(l))
                return l;
        }

        throw new InvalidOperationException();
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
        var result = new Day03().Prioritize(_example);

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
        new Day03().ValueItem(item).Should().Be(value);
    }

    
    [TestCase("vJrwpWtwJgWrhcsFMMfFFhFp", 'p')]
    [TestCase("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 'L')]
    [TestCase("PmmdzqPrVvPwwTWBwg", 'P')]
    [TestCase("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", 'v')]
    [TestCase("ttgJtRGJQctTZtZT", 't')]
    [TestCase("CrZsJsPPZsGzwwsLwLmpwMDw", 's')]
    public void GetItemFromBothCompartmentsTest(string rucksack, char item)
    {
        new Day03().GetItemFromBothCompartments(rucksack).Should().Be(item);
    }

    [Test]
    public void Part1Input()
    {
        var result = new Day03().Prioritize(Helper.ReadDay(3));

        result.Should().Be(0);
    }
}