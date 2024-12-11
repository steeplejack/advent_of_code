namespace Day11;

using System.Collections.Generic;
using static System.Math;

class Program
{
    static List<int> ReadInput(string filename)
    {
        List<int> input = new();
        using (StreamReader reader = new(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (var elem in line.Trim().Split())
                {
                    input.Add(int.Parse(elem));
                }
            }
        }
        return input;
    }

    static int Digits(long n)
    {
        return 1 + (int)Log10(n);
    }

    static long NumStones(long n, int depth)
    {
        Dictionary<(long, int), long> memo = new();

        long NumStonesRecursive(long n, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }

            if (memo.ContainsKey((n, depth)))
            {
                return memo[(n, depth)];
            }

            if (n == 0)
            {
                long val = NumStonesRecursive(1, depth - 1);
                memo.Add((n, depth), val);
                return val;
            }

            int d = Digits(n);
            if ((d & 1) == 1)
            {
                long val = NumStonesRecursive(n * 2024, depth - 1);
                memo.Add((n, depth), val);
                return val;
            }

            long remainder;
            long quotient = DivRem(n, (long)Pow(10, d / 2), out remainder);
            long value = NumStonesRecursive(quotient, depth - 1) + NumStonesRecursive(remainder, depth - 1);
            memo.Add((n, depth), value);
            return value;
        }

        return NumStonesRecursive(n, depth);
    }

    static void Main(string[] args)
    {
        List<int> input = ReadInput(args[1]);
        long part1 = input.Select(x => NumStones(x, 25)).Sum();
        long part2 = input.Select(x => NumStones(x, 75)).Sum();
        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
    }
}