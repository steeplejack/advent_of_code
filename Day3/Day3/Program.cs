namespace Day3;
using System.Text.RegularExpressions;

/*
 * The gist of part 1 is that you are given a bunch of strings that contain the pattern
 * "mul(N,M)", where N and M are integers, interspersed among a load of nonsense (Part 2
 * will probably reveal that the nonsense is no such thing, let's wait and see).
 * The task is to extract all matching patterns, calculate N*M for each one, and return
 * all these results added together.
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
    }

    static void Main(string[] args)
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
}