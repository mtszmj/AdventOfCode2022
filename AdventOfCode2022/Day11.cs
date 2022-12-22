namespace AdventOfCode2022;

public class Day11
{
    public long Part1(Monkey[] monkeys, int rounds)
    {
        for (var i = 0; i < rounds; i++)
        {
            Round(monkeys);
        }
        
        return monkeys.Select(x => x.Inspections).OrderDescending().Take(2).Aggregate(1L, (agg, x) => agg * x);
    }

    private void Round(Monkey[] monkeys)
    {
        foreach (var monkey in monkeys)
        {
            while (monkey.HasItem)
            {
                var (m,item) = monkey.Throw();
                monkeys[m].Catch(item);
            }
        }
    }
}

public class Monkey
{
    public Monkey(IEnumerable<long> items, Func<long, long> operation, Func<long, long> test)
    {
        foreach(var i in items)
            Items.Enqueue(i);

        Operation = operation;
        Test = test;
    }

    private Queue<long> Items { get; } = new();
    private Func<long, long> Operation { get; }
    private Func<long, long> Test { get; }
    private static Func<long, long> GetBored { get; } = x => x / 3;

    public long Inspections { get; private set; }

    public bool HasItem => Items.Any();
    public void Catch(long item) => Items.Enqueue(item);
    public (long Monkey, long Value) Throw()
    {
        Inspections++;
        var worry = Operation(Items.Dequeue());
        var boredWorry = GetBored(worry);
        var toMonkey = Test(boredWorry);
        return (toMonkey, boredWorry);
    }
}

[TestFixture]
public class Day11Tests
{
    [Test]
    public void Part1Example()
    {
        var monkeys = new Monkey[]
        {
            new (
                new long[] { 79, 98 },
                x => x * 19,
                x => x % 23 == 0 ? 2 : 3
                ),
            new (
                new long[] { 54, 65, 75, 74 },
                x => x + 6,
                x => x % 19 == 0 ? 2 : 0
            ),
            new (
                new long[] { 79, 60, 97 },
                x => x * x,
                x => x % 13 == 0 ? 1 : 3
            ),
            new (
                new long[] { 74 },
                x => x + 3,
                x => x % 17 == 0 ? 0 : 1
            ),
        };

        new Day11().Part1(monkeys, 20).Should().Be(10605);
    }

    [Test]
    public void Part1Input()
    {
        var monkeys = new Monkey[]
        {
            new (
                new long[] { 72, 64, 51, 57, 93, 97, 68 },
                x => x * 19,
                x => x % 17 == 0 ? 4 : 7
                ),
            new (
                new long[] { 62 },
                x => x * 11,
                x => x % 3 == 0 ? 3 : 2
            ),
            new (
                new long[] { 57, 94, 69, 79, 72 },
                x => x + 6,
                x => x % 19 == 0 ? 0 : 4
            ),
            new (
                new long[] { 80, 64, 92, 93, 64, 56 },
                x => x + 5,
                x => x % 7 == 0 ? 2 : 0
            ),
            new (
                new long[] { 70, 88, 95, 99, 78, 72, 65, 94 },
                x => x + 7,
                x => x % 2 == 0 ? 7 : 5
            ),
            new (
                new long[] { 57, 95, 81, 61 },
                x => x * x,
                x => x % 5 == 0 ? 1 : 6
            ),
            new (
                new long[] { 79, 99 },
                x => x + 2,
                x => x % 11 == 0 ? 3 : 1
            ),
            new (
                new long[] { 68, 98, 62 },
                x => x + 3,
                x => x % 13 == 0 ? 5 : 6
            ),
        };

        new Day11().Part1(monkeys, 20).Should().Be(99852);
    }

}