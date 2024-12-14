namespace Day13;

using System.Text.RegularExpressions;

internal class PuzzleValues(long a, long b, long c, long d, long m, long n)
{
    internal long a { get; } = a;
    internal long b { get; } = b;
    internal long c { get; } = c;
    internal long d { get; } = d;
    internal long m { get; } = m;
    internal long n { get; } = n;
}

class Program
{
    static PuzzleValues ExtractValues(List<string> data, long offset)
    {
        var values = Regex.Match(data[0], @"Button A: X\+(\d+), Y\+(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => long.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (a, c) = (values[0], values[1]);

        values = Regex.Match(data[1], @"Button B: X\+(\d+), Y\+(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => long.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (b, d) = (values[0], values[1]);

        values = Regex.Match(data[2], @"Prize: X=(\d+), Y=(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => long.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (m, n) = (values[0], values[1]);

        return new PuzzleValues(a, b, c, d, m + offset, n + offset);
    }

    static long? SolvePuzzle(PuzzleValues values)
    {
        var denom = 1.0 / (values.a * values.d - values.b * values.c);
        var x = double.Round((values.m * values.d - values.n * values.b) * denom);
        var y = double.Round((values.a * values.n - values.c * values.m) * denom);
        if (values.a * x + values.b * y == values.m && values.c * x + values.d * y == values.n)
        {
            return 3 * (long)x + (long)y;
        }
        return null;
    }

    static long ReadFile(string filename, long offset)
    {
        long sum = 0;
        using (StreamReader file = File.OpenText(filename))
        {
            string line;
            List<string> lines = new();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().Length > 0)
                {
                    lines.Add(line);
                }

                if (lines.Count == 3)
                {
                    var values = ExtractValues(lines, offset);
                    var solution = SolvePuzzle(values);
                    if (solution.HasValue)
                    {
                        sum += solution.Value;
                    }
                    lines.Clear();
                }
            }
        }

        return sum;
    }

    static void Main(string[] args)
    {
        var part1 = ReadFile(args[1], 0);
        var part2 = ReadFile(args[1], 10000000000000);
        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
    }
}