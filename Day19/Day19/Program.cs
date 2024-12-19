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

    static bool DynProg(List<string> patterns, string design, Dictionary<string, bool> cache)
    {
        if (cache.ContainsKey(design))
        {
            return cache[design];
        }

        foreach (var pattern in patterns)
        {
            if (pattern == design)
            {
                cache[design] = true;
                return true;
            }

            if (design.StartsWith(pattern))
            {
                if (DynProg(patterns, design.Substring(pattern.Length), cache))
                {
                    cache[design] = true;
                    return true;
                }
            }
        }
        cache[design] = false;
        return false;
    }

    static int Part1(List<string> patterns, List<string> designs)
    {
        return designs
            .Select<string, bool>(design =>
            {
                return DynProg(patterns, design, new Dictionary<string, bool>());
            })
            .Select(x => x ? 1 : 0)
            .Sum();
    }

    static void Main(string[] args)
    {
        var (patterns, designs) = ReadInput(args[1]);
        int part1 = Part1(patterns, designs);
        Console.WriteLine($"Part 1: {part1}");
    }
}