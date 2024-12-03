namespace Day3;
using System.Text.RegularExpressions;

/*
 * The gist of part 1 is that you are given a bunch of strings that contain the pattern
 * "mul(N,M)", where N and M are integers, interspersed among a load of nonsense (Part 2
 * will probably reveal that the nonsense is no such thing, let's wait and see).
 * The task is to extract all matching patterns, calculate N*M for each one, and return
 * all these results added together.
 *
 * Part 2: There are some other instructions strewn throughout the nonsense garbage. In
 * particular, the patterns "do()" and "don't()". Any "mul(N,M)"s encountered after a "don't()"
 * should be ignored, while any that come after a "do()" should count. Otherwise it's the same
 * as part 1 (but the total will end up being smaller).
 */
class Program
{
    static void Example()
    {
        Regex mulRegex = new Regex(@"(mul\((\d+),(\d+)\))", RegexOptions.Compiled);
        var example = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))\n";
        var matches = mulRegex.Matches(example);
        int s = 0;
        foreach (Match rgx in matches)
        {
            int a = int.Parse(rgx.Groups[2].Value);
            int b = int.Parse(rgx.Groups[3].Value);
            s += a * b;
        }
        Console.WriteLine(s);

        Regex regex = new Regex(@"(mul\((\d+),(\d+)\))|(do\(\))|(don't\(\))", RegexOptions.Compiled);
        var example2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
        var matches2 = regex.Matches(example2);
        int s2 = 0;
        bool do_ = true;
        foreach (Match rgx in matches2)
        {
            if (rgx.Groups[4].Value == "do()")
            {
                do_ = true;
            }

            if (rgx.Groups[5].Value == "don't()")
            {
                do_ = false;
            }

            if (rgx.Groups[1].Value.StartsWith("mul(") && do_)
            {
                int a = int.Parse(rgx.Groups[2].Value);
                int b = int.Parse(rgx.Groups[3].Value);
                s2 += a * b;
            }
        }
        Console.WriteLine(s2);
    }

    static void Part1(string[] args)
    {
        Regex mulRegex = new Regex(@"(mul\((\d+),(\d+)\))", RegexOptions.Compiled);
        int s = 0;
        using (StreamReader reader = new StreamReader(args[0]))
        {
            string line;
            while ((line = reader.ReadLine()) is not null)
            {
                var matches = mulRegex.Matches(line.Trim());
                foreach (Match match in matches)
                {
                    int a = int.Parse(match.Groups[2].Value);
                    int b = int.Parse(match.Groups[3].Value);
                    s += a * b;
                }
            }
        }
        Console.WriteLine(s);
    }

    static void Part2(string[] args)
    {
        Regex regex = new Regex(@"(mul\((\d+),(\d+)\))|(do\(\))|(don't\(\))", RegexOptions.Compiled);
        int s = 0;
        bool do_ = true;
        using (StreamReader reader = new StreamReader(args[0]))
        {
            string line;
            while ((line = reader.ReadLine()) is not null)
            {
                var matches = regex.Matches(line);
                foreach (Match match in matches)
                {
                    if (match.Groups[4].Value == "do()")
                    {
                        do_ = true;
                    }

                    if (match.Groups[5].Value == "don't()")
                    {
                        do_ = false;
                    }

                    if (match.Groups[1].Value.StartsWith("mul(") && do_)
                    {
                        int a = int.Parse(match.Groups[2].Value);
                        int b = int.Parse(match.Groups[3].Value);
                        s += a * b;
                    }
                }
            }
        }
        Console.WriteLine(s);
    }

    static void Main(string[] args)
    {
        Part1(args);
        Part2(args);
    }
}