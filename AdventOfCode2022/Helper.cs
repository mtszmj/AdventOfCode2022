namespace AdventOfCode2022;

public static class Helper
{
    public static string ReadDay(int day)
    {
        return File.ReadAllText($"Data/Day{day:00}.txt");
    }
}