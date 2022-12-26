using System.Numerics;

namespace AdventOfCode2022;

public class Day11
{
    public decimal Part1(Monkey[] monkeys, int rounds)
    {
        for (var i = 0; i < rounds; i++)
        {
            Round(monkeys);
        }

        Console.WriteLine(string.Join(",", monkeys.Select(x => x.Inspections)));
        
        return monkeys.Select(x => x.Inspections).OrderDescending().Take(2).Aggregate(1L, (agg, x) => agg * x);
    }

    public decimal Part2(Monkey[] monkeys, int rounds)
    {
        Monkey.GetBoredValue = 1;
        Monkey.MinifyValue = monkeys.Select(x => x.TestValue).Aggregate(1L, (agg, x) => agg * x);

        return Part1(monkeys, rounds);
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
    public Monkey(IEnumerable<long> items, Func<long, long> operation, Func<long, int> test, long testValue)
    {
        foreach(var i in items)
            Items.Enqueue(i);

        Operation = operation;
        Test = test;
        TestValue = testValue;
    }

    private Queue<long> Items { get; } = new();
    private Func<long, long> Operation { get; }
    private Func<long, int> Test { get; }
    
    public long TestValue { get; }
    private static Func<long, long> GetBored { get; } = x => x / GetBoredValue;

    public static long GetBoredValue = 3;
    public static long MinifyValue = long.MaxValue;
    
    public int Inspections { get; private set; }

    public bool HasItem => Items.Any();
    public void Catch(long item) => Items.Enqueue(item);
    public (int Monkey, long Value) Throw()
    {
        Inspections++;
        var worry = Operation(Items.Dequeue());
        var boredWorry = GetBored(worry);
        var toMonkey = Test(boredWorry);
        var newValue = boredWorry % MinifyValue;
        newValue = newValue == 0 ? boredWorry : newValue;
        return (toMonkey, newValue);
    }
}

[TestFixture]
public class Day11Tests
{
    [Test]
    public void Part1Example()
    {
        var monkeys = Example();

        new Day11().Part1(monkeys, 20).Should().Be(10605);
    }

    [Test]
    public void Part1Input()
    {
        var monkeys = Input();

        new Day11().Part1(monkeys, 20).Should().Be(99852);
    }

    [Test]
    public void Part2Example()
    {
        var monkeys = Example();

        new Day11().Part2(monkeys, 10000).Should().Be(2713310158L);
    }

    [Test]
    public void Part2Input()
    {
        var monkeys = Input();

        new Day11().Part2(monkeys, 10000).Should().Be(25935263541L);
    }

    private static Monkey[] Example()
    {
        var monkeys = new Monkey[]
        {
            new(
                new long[] { 79, 98 },
                x => x * 19,
                x => x % 23 == 0 ? 2 : 3,
                23
            ),
            new(
                new long[] { 54, 65, 75, 74 },
                x => x + 6,
                x => x % 19 == 0 ? 2 : 0,
                19
            ),
            new(
                new long[] { 79, 60, 97 },
                x => x * x,
                x => x % 13 == 0 ? 1 : 3,
                13
            ),
            new(
                new long[] { 74 },
                x => x + 3,
                x => x % 17 == 0 ? 0 : 1,
                17
            ),
        };
        return monkeys;
    }

    private static Monkey[] Input()
    {
        var monkeys = new Monkey[]
        {
            new(
                new long[] { 72, 64, 51, 57, 93, 97, 68 },
                x => x * 19,
                x => x % 17 == 0 ? 4 : 7,
                17
            ),
            new(
                new long[] { 62 },
                x => x * 11,
                x => x % 3 == 0 ? 3 : 2,
                3
            ),
            new(
                new long[] { 57, 94, 69, 79, 72 },
                x => x + 6,
                x => x % 19 == 0 ? 0 : 4,
                19
            ),
            new(
                new long[] { 80, 64, 92, 93, 64, 56 },
                x => x + 5,
                x => x % 7 == 0 ? 2 : 0,
                7
            ),
            new(
                new long[] { 70, 88, 95, 99, 78, 72, 65, 94 },
                x => x + 7,
                x => x % 2 == 0 ? 7 : 5,
                2
            ),
            new(
                new long[] { 57, 95, 81, 61 },
                x => x * x,
                x => x % 5 == 0 ? 1 : 6,
                5
            ),
            new(
                new long[] { 79, 99 },
                x => x + 2,
                x => x % 11 == 0 ? 3 : 1,
                11
            ),
            new(
                new long[] { 68, 98, 62 },
                x => x + 3,
                x => x % 13 == 0 ? 5 : 6,
                13
            ),
        };
        return monkeys;
    }
}