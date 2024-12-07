namespace Day7;

class Problem
{
    internal long target;
    internal long[] components;

    internal Problem(string line)
    {
        var split = line.Split(':');
        var target = split[0];
        var components = split[1];
        this.target = long.Parse(target);
        this.components = Array.ConvertAll(components.Trim().Split(' '), long.Parse);
    }
}

class Program
{
    static bool solve(long target, long[] components, bool part2)
    {
        int l = components.Length;
        bool inner(int index, long accumulator)
        {
            if (accumulator > target)
            {
                return false;
            }

            if (index == l)
            {
                return accumulator == target;
            }

            if (part2)
            {
                return inner(index + 1, accumulator * components[index]) ||
                       inner(index + 1, accumulator + components[index]) ||
                       inner(index + 1, long.Parse(String.Concat(accumulator, components[index])));
            }
            else
            {
                return inner(index + 1, accumulator * components[index]) ||
                       inner(index + 1, accumulator + components[index]);
            }
        }

        return inner(1, (long)components[0]);
    }

    static void Runner(string filename, bool part2)
    {
        using (StreamReader reader = new(filename))
        {
            string line;
            long acc = 0;
            while ((line = reader.ReadLine()) != null)
            {
                Problem problem = new Problem(line);
                if (solve(problem.target, problem.components, part2))
                {
                    acc += problem.target;
                }
            }
            Console.WriteLine(acc);
        }
    }

    static void Main(string[] args)
    {
        Runner(args[1], false);
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        Runner(args[1], true);
        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    }
}