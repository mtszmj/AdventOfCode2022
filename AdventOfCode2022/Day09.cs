using System.Diagnostics;

namespace AdventOfCode2022;

public class Day09
{
    public int Solve(string input)
    {
        var head = (Row:0, Col:0);
        var tail = (Row:0, Col:0);
        var tailVisited = new HashSet<(int, int)>();
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => x.Split(" ")))
        {
            for (var i = 0; i < int.Parse(line[1]); i++)
            {
                head = line[0] switch
                {
                    "U" => (head.Row - 1, head.Col),
                    "D" => (head.Row + 1, head.Col),
                    "R" => (head.Row, head.Col + 1),
                    "L" => (head.Row, head.Col - 1),
                };

                tail = (head.Row - tail.Row, head.Col - tail.Col) switch
                {
                    (2, 0) => (tail.Row+1, tail.Col),
                    (-2, 0) => (tail.Row-1, tail.Col),
                    (0, 2) => (tail.Row, tail.Col+1),
                    (0, -2) => (tail.Row, tail.Col-1),
                    (-2, 1) => (tail.Row-1, tail.Col+1),
                    (2, 1) => (tail.Row+1, tail.Col+1),
                    (-2, -1) => (tail.Row-1, tail.Col-1),
                    (2, -1) => (tail.Row+1, tail.Col-1),
                    (1, -2) => (tail.Row+1, tail.Col-1),
                    (1, 2) => (tail.Row+1, tail.Col+1),
                    (-1, -2) => (tail.Row-1, tail.Col-1),
                    (-1, 2) => (tail.Row-1, tail.Col+1),
                    _ => tail
                };

                tailVisited.Add(tail);
            }
        }

        return tailVisited.Count;
    }
}

[TestFixture]
public class Day09Tests
{
    private string _example = @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";

    [Test]
    public void Part1Example()
    {
        new Day09().Solve(_example).Should().Be(13);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day09().Solve(Helper.ReadDay(9)).Should().Be(5779);
    }
}