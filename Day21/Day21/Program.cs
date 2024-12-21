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

    static void Main(string[] args)
    {
        var input = ReadInput(args[1]);
        foreach (var line in input)
        {
            Console.WriteLine(line);
        }

        var numpad = new NumPad();
        var numpadPaths = numpad.GetPaths();

        var dirpad = new DirPad();
        var dirpadPaths = dirpad.GetPaths();

        long s = 0;

        foreach (var i in input)
        {
            if (i.Length == 0) continue;
            int n = int.Parse(i.Substring(0, 3));
            var shortestPaths1 = ShortestPaths(i, numpadPaths);
            var shortestPaths2 = shortestPaths1.SelectMany(p => ShortestPaths(p, dirpadPaths)).ToList();
            var shortestPaths3 = shortestPaths2.SelectMany(p => ShortestPaths(p, dirpadPaths)).Min(s => s.Length);

            s += (long)(shortestPaths3 * n);
        }
        Console.WriteLine(s);
    }

    static List<string> ShortestPaths(string sequence, Dictionary<(char, char), List<List<Node<(int, int, char)>>>> paths)
    {
        return Rec(("A" + sequence).ToCharArray(), 1, paths, "", new Dictionary<(char[], int), List<string>>());
    }

    static List<string> Rec(char[] sequence, int index, Dictionary<(char, char), List<List<Node<(int, int, char)>>>> paths, string acc, Dictionary<(char[], int), List<string>> cache)
    {
        if (cache.ContainsKey((sequence, index)))
        {
            return cache[(sequence, index)];
        }
        List<string> output = new();
        if (index == sequence.Length - 1)
        {
            foreach (var path in paths[(sequence[index - 1], sequence[index])])
            {
                output.Add(acc + KeyPad.PathToString(path));
            }

            cache.Add((sequence, index), output);
            return output;
        }
        else
        {
            foreach (var path in paths[(sequence[index - 1], sequence[index])])
            {
                output.AddRange(Rec(sequence, index + 1, paths, acc + KeyPad.PathToString(path), cache));
            }

            cache.Add((sequence, index), output);
            return output;
        }
    }
}