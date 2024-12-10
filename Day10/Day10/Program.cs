namespace Day10;
using Graph = List<List<Node>>;

internal class Node(int elevation, int row, int col): IEquatable<Node>
{
    internal int elevation = elevation;
    internal int row = row;
    internal int col = col;
    internal List<Node> children = new();

    internal void AddChild(Node node) => children.Add(node);

    public override bool Equals(object obj)
    {
        return Equals(obj as Node);
    }

    public bool Equals(Node other)
    {
        return elevation == other.elevation;
    }

    public override int GetHashCode()
    {
        return (10000 * elevation).GetHashCode() ^ (100*row).GetHashCode() ^ col.GetHashCode();
    }
}

class Program
{
    static int DFS(Graph graph, Node start, int target)
    {
        Stack<Node> stack = new();
        stack.Push(start);
        HashSet<Node> results = new();

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node.elevation == target)
            {
                results.Add(node);
            }

            foreach (var child in node.children)
            {
                stack.Push(child);
            }
        }

        return results.Count;
    }
    static void Main(string[] args)
    {
        var (graph, nrow, ncol) = ReadGraph(args[1]);
        ConnectGraph(graph, nrow, ncol);
        var (starts, ends) = FindStartsAndEnds(graph, nrow, ncol);
        int sum = starts.Select(x => DFS(graph, x, 9)).Sum();
        Console.WriteLine(sum);
    }
    static (Graph, int, int) ReadGraph(string filename)
    {
        List<List<Node>> graph = new();
        int nrow = 0;
        int ncol = 0;
        using (StreamReader file = File.OpenText(filename))
        {
            string line;
            while ((line = file.ReadLine()) != null)
            {
                int i = 0;
                List<Node> row = new();
                foreach (char c in line.Trim().ToCharArray())
                {
                    int elevation = int.Parse(c.ToString());
                    Node newNode = new Node(elevation, nrow, i);
                    row.Add(newNode);
                    i++;
                    if (i > ncol) ncol = i;
                }

                graph.Add(row);
                nrow++;
            }
        }
        return (graph, nrow, ncol);
    }

    static void ConnectGraph(Graph graph, int nrow, int ncol)
    {
        for (int i = 0; i < nrow - 0; i++)
        {
            for (int j = 0; j < ncol - 0; j++)
            {
                var n = graph[i][j];

                List<Node> neighbours = new();

                if (i > 0) neighbours.Add(graph[i - 1][j]);
                if (i + 1 < nrow) neighbours.Add(graph[i + 1][j]);
                if (j > 0) neighbours.Add(graph[i][j - 1]);
                if (j + 1 < ncol) neighbours.Add(graph[i][j + 1]);

                foreach (var neighbour in neighbours.ToArray())
                {
                    if (neighbour.elevation - n.elevation == 1)
                    {
                        n.children.Add(neighbour);
                    }
                }
            }
        }
    }

    static (List<Node>, List<Node>) FindStartsAndEnds(Graph graph, int nrow, int ncol)
    {
        var starts = new List<Node>();
        var ends = new List<Node>();
        for (int i = 0; i < nrow; i++)
        {
            for (int j = 0; j < ncol; j++)
            {
                if (graph[i][j].elevation == 0)
                {
                    starts.Add(graph[i][j]);
                }

                if (graph[i][j].elevation == 9)
                {
                    ends.Add(graph[i][j]);
                }
            }
        }
        return (starts, ends);
    }
}