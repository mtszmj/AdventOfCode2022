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
    
    public int Part2(string input, int ropeSize, bool print = false)
    {
        var knots = new Knot[ropeSize];
        var tailVisited = new HashSet<Knot>();

        var mapSize = (-1, 1, -1, 1);
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => x.Split(" ")))
        {
            for (var i = 0; i < int.Parse(line[1]); i++)
            {
                knots[0] = PositionHead(knots[0] ?? new Knot(0,0), line[0]);
                for (var k = 0; k < ropeSize - 1; k++)
                {
                    knots[k + 1] = PositionTail(knots[k] ?? new Knot(0,0), knots[k + 1] ?? new Knot(0,0));
                }
                tailVisited.Add(knots[ropeSize - 1]);
            }

            if(print)
                mapSize = Print(knots, ropeSize, mapSize);
        }

        return tailVisited.Count;
    }

    public (int, int, int, int) Print(Knot[] knots, int ropeSize, (int minR, int maxR, int minC, int maxC) mapSize)
    {
        foreach (var k in knots)
        {
            mapSize = (
                Math.Min(mapSize.minR,k.Row), 
                Math.Max(mapSize.maxR, k.Row), 
                Math.Min(mapSize.minC, k.Col),
                Math.Max(mapSize.maxC, k.Col)
                );
        }

        var map = new char[mapSize.maxR - mapSize.minR + 1, mapSize.maxC - mapSize.minC + 1];
        for (var k = ropeSize - 1; k >= 0; k--)
        {
            map[knots[k].Row-mapSize.minR, knots[k].Col-mapSize.minC] = (char)(k + 48);
        }

        for (var r = mapSize.minR; r <= mapSize.maxR; r++)
        {
            for (var c = mapSize.minC; c <= mapSize.maxC; c++)
            {
                Console.Write(map[r-mapSize.minR,c-mapSize.minC] == 0 ? '#' : map[r-mapSize.minR,c-mapSize.minC]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        return mapSize;
    }

    private Knot PositionHead(Knot head, string command)
    {
        return command switch
        {
            "U" => head with { Row = head.Row - 1 },
            "D" => head with { Row = head.Row + 1 },
            "R" => head with { Col = head.Col + 1 },
            "L" => head with { Col = head.Col - 1 }
        };
    }

    private Knot PositionTail(Knot head, Knot tail)
    {
        return (head.Row - tail.Row, head.Col - tail.Col) switch
        {
            (2, 0) => tail with { Row = tail.Row + 1 },
            (-2, 0) => tail with { Row = tail.Row - 1 },
            (0, 2) => tail with { Col = tail.Col + 1 },
            (0, -2) => tail with { Col = tail.Col - 1 },
            (-2, 1) => new Knot(tail.Row - 1, tail.Col + 1),
            (2, 1) => new Knot(tail.Row + 1, tail.Col + 1),
            (-2, -1) => new Knot(tail.Row - 1, tail.Col - 1),
            (2, -1) => new Knot(tail.Row + 1, tail.Col - 1),
            (1, -2) => new Knot(tail.Row + 1, tail.Col - 1),
            (1, 2) => new Knot(tail.Row + 1, tail.Col + 1),
            (-1, -2) => new Knot(tail.Row - 1, tail.Col - 1),
            (-1, 2) => new Knot(tail.Row - 1, tail.Col + 1),
            (-2, 2) => new Knot(tail.Row - 1, tail.Col + 1),
            (2, 2) => new Knot(tail.Row + 1, tail.Col + 1),
            (2, -2) => new Knot(tail.Row + 1, tail.Col - 1),
            (-2, -2) => new Knot(tail.Row - 1, tail.Col - 1),
            _ => tail
        };
    }
}

public record Knot(int Row, int Col);

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

    private string _example2 = @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20";
    
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
    
    [Test]
    public void Part2Example1()
    {
        new Day09().Part2(_example,2).Should().Be(13);
    }
    
    [Test]
    public void Part2Input1()
    {
        new Day09().Part2(Helper.ReadDay(9),2).Should().Be(5779);
    }
    
    [Test]
    public void Part2Example()
    {
        new Day09().Part2(_example2, 10).Should().Be(36);
    }   
    
    [Test]
    public void Part2Input()
    {
        new Day09().Part2(Helper.ReadDay(9),10).Should().Be(2331);
    }
}