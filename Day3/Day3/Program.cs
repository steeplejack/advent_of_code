namespace Day3;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
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
}