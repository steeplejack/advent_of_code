// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

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
    
    public static void Main(string[] args)
    {
        StreamReader reader = new StreamReader(args[0]);
        string line;
        int s = 0;
        while ((line = reader.ReadLine()) != null)
        {
            var numbers = line.Trim()
                .Split(' ')
                .Select(a => int.Parse(a))
                .ToArray();
            if (IsSafe(numbers))
            {
                s += 1;
            }
        }
        Console.WriteLine(s);
    }
}