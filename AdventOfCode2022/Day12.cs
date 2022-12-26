namespace AdventOfCode2022;

public class Day12
{
    public int Part1(string input)
    {
        var map = MapTiles(input, out var queue, false);
        return Solve(queue, map);
    }

    public int Part2(string input)
    {
        var map = MapTiles(input, out var queue, true);
        return Solve(queue, map);
    }

    private static int Solve(PriorityQueue<MapTile, int> queue, MapTile[,] map)
    {
        var iter = 0;
        while (true)
        {
            if (!queue.TryDequeue(out var current, out var currentMoves))
                throw new InvalidOperationException($"No data in queue!? Iterations: {iter}");

            // Console.WriteLine($"[{current.Row},{current.Col}]");
            iter++;

            if (current.Visited)
                continue;

            if (current.IsEnd)
            {
                Console.WriteLine($"Iterations: {iter}");
                return currentMoves;
            }

            current.Visited = true;

            var moves = new[]
            {
                (current.Row - 1, current.Col),
                (current.Row + 1, current.Col),
                (current.Row, current.Col - 1),
                (current.Row, current.Col + 1)
            };
            foreach (var move in moves)
            {
                if (move.Item1 < 0 || move.Item2 < 0 || move.Item1 >= map.GetLength(0) ||
                    move.Item2 >= map.GetLength(1))
                    continue;

                if (map[move.Item1, move.Item2].Visited) continue;

                if (map[move.Item1, move.Item2].Value > current.Value + 1) continue;

                queue.Enqueue(map[move.Item1, move.Item2], currentMoves + 1);
            }
        }
    }

    private static MapTile[,] MapTiles(string input, out PriorityQueue<MapTile, int> queue, bool allTilesWithValue1)
    {
        var i = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var map = new MapTile[i.Length, i[0].Length];
        queue = new PriorityQueue<MapTile, int>();
        for (var row = 0; row < i.Length; row++)
        {
            var line = i[row];
            for (var col = 0; col < line.Length; col++)
            {
                var c = line[col];
                if (c == 'S')
                {
                    map[row, col] =  new MapTile(row, col, 1, false, false);
                    queue.Enqueue(map[row, col], 0);
                }
                else if (c == 'E')
                {
                    map[row, col] = new MapTile(row, col, 'z' - 'a' + 1, false, true);
                }
                else
                {
                    map[row, col] = new MapTile(row, col, c - 'a' + 1, false, false);
                    if (allTilesWithValue1 && map[row, col].Value == 1)
                    {
                        queue.Enqueue(map[row, col], 0);
                    }
                }
            }
        }

        return map;
    }
}

public class MapTile
{
    public int Row;
    public int Col;
    public int Value;
    public bool Visited;
    public bool IsEnd;

    public MapTile(int row, int col, int value, bool visited, bool isEnd)
    {
        Row = row;
        Col = col;
        Value = value;
        Visited = visited;
        IsEnd = isEnd;
    }
}

[TestFixture]
public class Day12Tests
{
    [Test]
    public void Part1Example()
    {
        var input = @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";

        new Day12().Part1(input).Should().Be(31);
    }

    [Test]
    public void Part1Input()
    {
        new Day12().Part1(Helper.ReadDay(12)).Should().Be(423);
    }
    
    [Test]
    public void Part2Example()
    {
        var input = @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";

        new Day12().Part2(input).Should().Be(29);
    }

    [Test]
    public void Part2Input()
    {
        new Day12().Part2(Helper.ReadDay(12)).Should().Be(416);
    }
}