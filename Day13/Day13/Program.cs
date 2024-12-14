namespace Day13;

using System.Text.RegularExpressions;

internal class PuzzleValues(int a, int b, int c, int d, int m, int n)
{
    internal int a { get; } = a;
    internal int b { get; } = b;
    internal int c { get; } = c;
    internal int d { get; } = d;
    internal int m { get; } = m;
    internal int n { get; } = n;
}

class Program
{
    static PuzzleValues ExtractValues(List<string> data)
    {
        var values = Regex.Match(data[0], @"Button A: X\+(\d+), Y\+(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => int.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (a, c) = (values[0], values[1]);

        values = Regex.Match(data[1], @"Button B: X\+(\d+), Y\+(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => int.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (b, d) = (values[0], values[1]);

        values = Regex.Match(data[2], @"Prize: X=(\d+), Y=(\d+)")
            .Groups.Values
            .Skip(1)
            .Select(x => int.Parse(x.Value))
            .Take(2)
            .ToArray();
        var (m, n) = (values[0], values[1]);

        return new PuzzleValues(a, b, c, d, m, n);
    }

    static int? SolvePuzzle(PuzzleValues values)
    {
        var denom = 1.0 / (values.a * values.d - values.b * values.c);
        var x = double.Round((values.m * values.d - values.n * values.b) * denom);
        var y = double.Round((values.a * values.n - values.c * values.m) * denom);
        if (values.a * x + values.b * y == values.m && values.c * x + values.d * y == values.n)
        {
            return 3 * (int)x + (int)y;
        }
        return null;
    }

    static int ReadFile(string filename)
    {
        int sum = 0;
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
                    var values = ExtractValues(lines);
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
        var part1 = ReadFile(args[1]);
        Console.WriteLine($"Part 1: {part1}");
    }
}