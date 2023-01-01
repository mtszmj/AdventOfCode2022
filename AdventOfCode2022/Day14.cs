using System.Text;

namespace AdventOfCode2022;

public class Day14
{
    public int Part1(string input, bool withPrint = false)
    {
        var mapPoints = FillMap(input);

        var minCol = mapPoints.MinBy(x => x.Col).Col;
        var maxCol = mapPoints.MaxBy(x => x.Col).Col;
        var minRow = mapPoints.MinBy(x => x.Row).Row;
        var maxRow = mapPoints.MaxBy(x => x.Row).Row;

        var mapCopy = new HashSet<(int Col, int Row)>(mapPoints);

        Print(mapPoints, mapCopy, withPrint);
        
        var sand = 1;
        var sandPos = (Col: 500, Row: 0);
        while (true)
        {
            if (!mapPoints.Contains((sandPos.Col, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col, sandPos.Row + 1);
                if (sandPos.Row > maxRow)
                {
                    break;
                }
            }
            else if (!mapPoints.Contains((sandPos.Col - 1, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col - 1, sandPos.Row + 1);
            }
            else if (!mapPoints.Contains((sandPos.Col + 1, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col + 1, sandPos.Row + 1);
            }
            else //rest
            {
                mapPoints.Add(sandPos);
                
                //next sand unit
                if(withPrint)
                    Console.WriteLine($"Unit: {sand}");
                Print(mapPoints, mapCopy, withPrint);
                sand++;
                sandPos = (Col: 500, Row: 0);
            }
        }

        return sand - 1;
    }

    public int Part2(string input, bool withPrint = false)
    {
        var mapPoints = FillMap(input);
        var minCol = mapPoints.MinBy(x => x.Col).Col;
        var maxCol = mapPoints.MaxBy(x => x.Col).Col;
        var minRow = mapPoints.MinBy(x => x.Row).Row;
        var maxRow = mapPoints.MaxBy(x => x.Row).Row + 2;

        var mapCopy = new HashSet<(int Col, int Row)>(mapPoints);

        Print(mapPoints, mapCopy, withPrint);
        
        var sand = 1;
        var sandPos = (Col: 500, Row: 0);
        while (true)
        {
            if (sandPos.Row == maxRow - 1)
            {
                //rest
                mapPoints.Add(sandPos);

                //next sand unit
                if (withPrint)
                    Console.WriteLine($"Unit: {sand}");
                Print(mapPoints, mapCopy, withPrint);
                sand++;
                sandPos = (Col: 500, Row: 0);
            }
            else if (!mapPoints.Contains((sandPos.Col, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col, sandPos.Row + 1);
            }
            else if (!mapPoints.Contains((sandPos.Col - 1, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col - 1, sandPos.Row + 1);
            }
            else if (!mapPoints.Contains((sandPos.Col + 1, sandPos.Row + 1)))
            {
                sandPos = (sandPos.Col + 1, sandPos.Row + 1);
            }
            else if (sandPos == (500, 0))
            {
                mapPoints.Add(sandPos);
                if(withPrint)
                    Console.WriteLine($"Unit: {sand}");
                Print(mapPoints, mapCopy, withPrint);
                break;
            }
            else //rest
            {
                mapPoints.Add(sandPos);
                
                //next sand unit
                if(withPrint)
                    Console.WriteLine($"Unit: {sand}");
                Print(mapPoints, mapCopy, withPrint);
                sand++;
                sandPos = (Col: 500, Row: 0);
            }
        }

        return sand;
    }

    private void Print(HashSet<(int Col, int Row)> mapPoints, HashSet<(int Col, int Row)> mapCopy, bool withPrint)
    {
        if (!withPrint)
            return;
        
        var minCol = mapPoints.MinBy(x => x.Col).Col;
        var maxCol = mapPoints.MaxBy(x => x.Col).Col;
        var minRow = 0;
        var maxRow = mapPoints.MaxBy(x => x.Row).Row;

        var sb = new StringBuilder();
        for (var row = minRow; row <= maxRow; row++)
        {
            for (var col = minCol; col <= maxCol; col++)
            {
                if (row == 0 && col == 500)
                {
                    sb.Append('+');
                    continue;
                }
                if (mapCopy.Contains((col, row)))
                {
                    sb.Append('#');
                }
                else if (mapPoints.Contains((col, row)))
                {
                    sb.Append('o');
                }
                else
                {
                    sb.Append('.');
                }
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    private static HashSet<(int Col, int Row)> FillMap(string input)
    {
        var mapPoints = new HashSet<(int Col, int Row)>();
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var points = line.Split(" -> ").Select(x => x.Split(","))
                .Select(x => (Col: int.Parse(x[0]), Row: int.Parse(x[1])))
                .ToArray();

            for (var i = 0; i < points.Length - 1; i++)
            {
                var colRise = points[i].Col <= points[i + 1].Col;
                var colAdd = colRise ? 1 : -1;
                var rowRise = points[i].Row <= points[i + 1].Row;
                var rowAdd = rowRise ? 1 : -1;

                for (var col = points[i].Col;
                     (colRise && col <= points[i + 1].Col) ||
                     (!colRise && col >= points[i + 1].Col);
                     col += colAdd)
                {
                    for (var row = points[i].Row;
                         (rowRise && row <= points[i + 1].Row) ||
                         (!rowRise && row >= points[i + 1].Row);
                         row += rowAdd)
                    {
                        mapPoints.Add((col, row));
                    }
                }
            }
        }

        return mapPoints;
    }
}

[TestFixture]
public class Day14Tests
{
    public const string Example = @"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9";

    [Test]
    public void Part1Example()
    {
        new Day14().Part1(Example, true).Should().Be(24);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day14().Part1(Helper.ReadDay(14), false).Should().Be(901);
    }
    
    [Test]
    public void Part2Example()
    {
        new Day14().Part2(Example, true).Should().Be(93);
    }
    
    [Test]
    public void Part2Input()
    {
        new Day14().Part2(Helper.ReadDay(14), false).Should().Be(24589);
    }
}