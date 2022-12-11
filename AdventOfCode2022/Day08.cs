using System.Runtime.CompilerServices;

namespace AdventOfCode2022;

public class Day08
{
    public int Part1(string input)
    {
        var trees = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(c => int.Parse($"{c}")).ToArray()).ToArray();

        var rowsCount = trees.GetLength(0);
        var colsCount = trees[0].GetLength(0);
        
        var isVisible = new bool[rowsCount][];
        
        for (var row = 0; row < rowsCount; row++)
        {
            isVisible[row] = new bool[colsCount];
        }

        for (var row = 0; row < rowsCount; row++)
        {
            var maxHeight = -1;
            for (var col = 0; col < colsCount; col++)
            {
                if (maxHeight < trees[row][col])
                {
                    maxHeight = trees[row][col];
                    isVisible[row][col] = true;
                    if (maxHeight == 9)
                        break;
                }
            }
        }
        
        for (var row = 0; row < rowsCount; row++)
        {
            var maxHeight = -1;
            for (var col = colsCount - 1; col >= 0; col--)
            {
                if (maxHeight < trees[row][col])
                {
                    maxHeight = trees[row][col];
                    isVisible[row][col] = true;
                    if (maxHeight == 9)
                        break;
                }
            }
        }
        
        for (var col = 0; col < colsCount; col++)
        {
            var maxHeight = -1;
            for (var row = 0; row < rowsCount; row++)
            {
                if (maxHeight < trees[row][col])
                {
                    maxHeight = trees[row][col];
                    isVisible[row][col] = true;
                    if (maxHeight == 9)
                        break;
                }
            }
        }
        
        for (var col = 0; col < colsCount; col++)
        {
            var maxHeight = -1;
            for (var row = rowsCount - 1; row >= 0; row--)
            {
                if (maxHeight < trees[row][col])
                {
                    maxHeight = trees[row][col];
                    isVisible[row][col] = true;
                    if (maxHeight == 9)
                        break;
                }
            }
        }

        Print(trees, isVisible);

        return isVisible.Select(x => x).SelectMany(x => x).Count(x => x);
    }

    private void Print(int[][] trees, bool[][] isVisible)
    {
        var rowsCount = trees.GetLength(0);
        var colsCount = trees[0].GetLength(0);

        for (var row = 0; row < rowsCount; row++)
        {
            for (var col = 0; col < colsCount; col++)
            {
                if (isVisible[row][col])
                    Console.ForegroundColor = ConsoleColor.White;
                else Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(trees[row][col]);
            }
            Console.WriteLine();
        }
    }
}

[TestFixture]
public class Day08Tests
{
    private string _example = @"30373
25512
65332
33549
35390";

    [Test]
    public void Part1Example()
    {
        new Day08().Part1(_example).Should().Be(21);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day08().Part1(Helper.ReadDay(8)).Should().Be(1698);
    }

}