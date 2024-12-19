namespace Day19;

class Program
{
    static (List<string>, List<string>) ReadInput(string filename)
    {
        List<String> patterns;
        List<string> designs = new List<string>();
        using (StreamReader reader = File.OpenText(filename))
        {
            string line;
            patterns = reader.ReadLine().Trim().Split(',').Select(x => x.Trim()).ToList();
            line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                designs.Add(line.Trim());
            }
        }

        return (patterns, designs);
    }

    static long DynProg(List<string> patterns, string design, Dictionary<string, long> cache)
    {
        if (cache.ContainsKey(design))
        {
            return cache[design];
        }

        long acc = 0;
        foreach (var pattern in patterns)
        {
            if (pattern == design)
            {
                acc++;
            }

            if (design.StartsWith(pattern))
            {
                acc += DynProg(patterns, design.Substring(pattern.Length), cache);
            }
        }

        cache[design] = acc;
        return acc;
    }

    static int Part1(List<string> patterns, List<string> designs)
    {
        return designs
            .Select<string, long>(design => DynProg(patterns, design, new Dictionary<string, long>()))
            .Select(x => x > 0 ? 1 : 0)
            .Sum();
    }

    static long Part2(List<string> patterns, List<string> designs)
    {
        return designs
            .Select(design => DynProg(patterns, design, new Dictionary<string, long>()))
            .Sum();
    }

    static void Main(string[] args)
    {
        var (patterns, designs) = ReadInput(args[1]);
        int part1 = Part1(patterns, designs);
        long part2 = Part2(patterns, designs);
        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
    }
}