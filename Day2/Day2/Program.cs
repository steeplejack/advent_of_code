internal class Program
{
    static int[] Diff(int[] input)
    {
        return input
            .SkipLast(1)
            .Zip(input.Skip(1),
                (f, l) => l - f)
            .ToArray();
    }

    static bool IsSafe(int[] input)
    {
        var diff = Diff(input);
        var sign = Math.Sign(diff[0]);
        return diff.All(i =>
        { 
            int abs = Math.Abs(i);
            return (Math.Sign(i) == sign) && abs > 0 && abs < 4;
        });
    }

    static bool IsSafeWithDamping(int[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            var skipped = input.Where((ValueTask, index) => index != i).ToArray();
            if (IsSafe(skipped))
            {
                return true;
            }
        }

        return false;
    }
    
    public static void Main(string[] args)
    {
        StreamReader reader = new StreamReader(args[0]);
        string line;
        int part1 = 0;
        int part2 = 0;
        while ((line = reader.ReadLine()) != null)
        {
            var numbers = line.Trim()
                .Split(' ')
                .Select(a => int.Parse(a))
                .ToArray();

            if (IsSafe(numbers))
            {
                part1 += 1;
                part2 += 1;
            }

            else if (IsSafeWithDamping(numbers))
            {
                part2 += 1;
            }

        }
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}