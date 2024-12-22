namespace Day21;

class DirPad : KeyPad
{
    protected override (int, int, char)[] _nodeValues => new (int, int, char)[]
    {
        (0, 1, '^'), (0, 2, 'A'), (1, 0, '<'), (1, 1, 'v'), (1, 2, '>')
    };
}

class NumPad : KeyPad
{
    protected override (int, int, char)[] _nodeValues => new (int, int, char)[]
    {
        (0,0,'7'),
        (0,1,'8'),
        (0,2,'9'),
        (1,0,'4'),
        (1,1,'5'),
        (1,2,'6'),
        (2,0,'1'),
        (2,1,'2'),
        (2,2,'3'),
        (3,1,'0'),
        (3,2,'A')
    };
}

class KeyPad
{
    protected virtual (int, int, char)[] _nodeValues => new (int, int, char)[]
    {
        (0,0,'7'),
        (0,1,'8'),
        (0,2,'9'),
        (1,0,'4'),
        (1,1,'5'),
        (1,2,'6'),
        (2,0,'1'),
        (2,1,'2'),
        (2,2,'3'),
        (3,1,'0'),
        (3,2,'A')
    };

    public Graph<(int,int,char)> Graph { get; }

    private Dictionary<(char, char), List<List<Node<(int, int, char)>>>> _paths = new();

    public KeyPad()
    {
        // initialise the graph
        Graph = new();

        // add nodes to the graph
        foreach (var node in _nodeValues)
        {
            Graph.AddNode(node);
        }

        // add edges to the graph
        foreach (var source in _nodeValues)
        {
            foreach (var dest in _nodeValues)
            {
                if (source == dest)
                {
                    continue;
                }

                if (Math.Abs(source.Item1 - dest.Item1) + Math.Abs(source.Item2 - dest.Item2) ==
                    1)
                {
                    Graph.AddEdge(source, dest, 1.0);
                }
            }
        }

        // collect paths
        foreach (var source in _nodeValues)
        {
            var sourceNode = Graph.GetNode(source);
            foreach (var dest in _nodeValues)
            {
                var destNode = Graph.GetNode(dest);
                var (_, paths) = Graph.DijkstraPath(sourceNode, destNode);
                _paths.Add((source.Item3, dest.Item3), paths);
            }
        }
    }

    public Graph<(int, int, char)> GetGraph() => Graph;
    public Dictionary<(char, char), List<List<Node<(int, int, char)>>>> GetPaths() => _paths;

    public static string PathToString(List<Node<(int, int, char)>> path)
    {
        if (path.Count == 0)
        {
            return "";
        }

        if (path.Count == 1)
        {
            return "A";
        }

        var output = "";
        for (int i = 1; i < path.Count; i++)
        {
            var from = path[i - 1];
            var to = path[i];
            var vDiff = to.Value.Item1 - from.Value.Item1;
            var hDiff = to.Value.Item2 - from.Value.Item2;
            output += (vDiff, hDiff) switch
            {
                (1, 0) => "v",
                (-1, 0) => "^",
                (0, 1) => ">",
                (0, -1) => "<",
                _ => throw new InvalidOperationException("Invalid path"),
            };

        }

        return output + "A";
    }
}


class Program
{
    static string[] ReadInput(string filename)
    {
        using (var reader = File.OpenText(filename))
        {
            return reader.ReadToEnd().Split('\n');
        }
    }

    static long MinPath(int level, int layers, string input,
        Dictionary<(char, char), List<string>> numPaths,
        Dictionary<(char, char), List<string>> dirPaths,
        Dictionary<(int, string), long> cache)
    {
        if (cache.ContainsKey((level, input)))
        {
            return cache[(level, input)];
        }

        if (level == layers + 1)
        {
            cache.Add((level, input), input.Length);
            return (long)input.Length;
        }

        var paths = level == 0 ? numPaths : dirPaths;

        long total = 0;
        for (int i = 0; i < input.Length; i++)
        {
            char start = default;
            if (i == 0)
            {
                start = 'A';
            }
            else
            {
                start = input[i - 1];
            }
            char end = input[i];
            List<long> scores = new();
            paths[(start, end)].ForEach(path =>
            {
                scores.Add(MinPath(level + 1, layers, path, numPaths, dirPaths, cache));
            });
            total += scores.Min();
        }

        cache.Add((level, input), total);
        return total;
    }

    static void Main(string[] args)
    {
        var input = ReadInput(args[1]);
        foreach (var line in input)
        {
            Console.WriteLine(line);
        }

        var numpad = new NumPad();
        var numpadPaths = numpad.GetPaths();
        var numpadPathsS = new Dictionary<(char, char), List<string>>();
        foreach (var kvp in numpadPaths)
        {
            var stringList = kvp.Value.Select(x => NumPad.PathToString(x)).ToList();
            numpadPathsS[kvp.Key] = stringList;
        }

        var dirpad = new DirPad();
        var dirpadPaths = dirpad.GetPaths();
        var dirpadPathsS = new Dictionary<(char, char), List<string>>();
        foreach (var kvp in dirpadPaths)
        {
            var stringList = kvp.Value.Select(x => DirPad.PathToString(x)).ToList();
            dirpadPathsS[kvp.Key] = stringList;
        }

        long result = 0;
        foreach (var inputCode in input)
        {
            if (inputCode.Length == 0) continue;
            long s = MinPath(0, 25, inputCode, numpadPathsS, dirpadPathsS, new Dictionary<(int, string), long>());
            long n = long.Parse(inputCode.Substring(0, inputCode.Length - 1));
            result += s * n;
        }

        Console.WriteLine(result);
    }


}