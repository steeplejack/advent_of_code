namespace Day16;

public class Node<T>
{
    public T Value { get; set; }

    public Node(T value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"Node({Value.ToString()})";
    }

    public override bool Equals(object obj)
    {
        return obj is Node<T> node && EqualityComparer<T>.Default.Equals(Value, node.Value);
    }

    public bool Equals(Node<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public static bool operator ==(Node<T> left, Node<T> right)
    {
        if (left is null && right is null) { return true; }
        if (left is null || right is null) { return false; }
        return left.Equals(right);
    }

    public static bool operator !=(Node<T> left, Node<T> right)
    {
        return !(left == right);
    }
}

public class Graph<T> where T : notnull
{
    public Dictionary<T, Node<T>> Nodes { get; }
    public Dictionary<Node<T>, Dictionary<Node<T>, double>> Edges { get; }

    public Graph()
    {
        Nodes = new Dictionary<T, Node<T>>();
        Edges = new Dictionary<Node<T>, Dictionary<Node<T>, double>>();
    }

    public void AddNode(T value)
    {
        if (Nodes.ContainsKey(value))
        {
            throw new ArgumentException($"Node already exists: {value}");
        }
        Nodes[value] = new Node<T>(value);
    }

    public Node<T>[] GetNodes()
    {
        return Nodes.Values.OrderBy(n => n.Value).ToArray();
    }

    public Node<T> GetNode(T value)
    {
        if (Nodes.TryGetValue(value, out var node))
        {
            return node;
        }
        else
        {
            throw new ArgumentException($"Node not found: {value}");
        }
    }

    public void AddEdge(T head, T tail, double distance)
    {
        var head_node = GetNode(head);
        var tail_node = GetNode(tail);

        if (!Edges.ContainsKey(head_node))
        {
            Edges[head_node] = new Dictionary<Node<T>, double>();
        }
        Edges[head_node][tail_node] = distance;

        if (!Edges.ContainsKey(tail_node))
        {
            Edges[tail_node] = new Dictionary<Node<T>, double>();
        }
        Edges[tail_node][head_node] = distance;
    }

    public List<(Node<T>, double)> GetNeighbours(Node<T> node)
    {
        var neighbours = new List<(Node<T>, double)>();
        foreach (var kvp in Edges[node])
        {
            neighbours.Add((kvp.Key, kvp.Value));
        }
        return neighbours;
    }

    public Dictionary<Node<T>, double> Dijkstra(Node<T> start)
    {
        if (!Nodes.Values.Contains(start))
        {
            throw new ArgumentException($"Start node not found: {start}");
        }
        PriorityQueue<(Node<T>, double), double> queue = new PriorityQueue<(Node<T>, double), double>();
        queue.Enqueue((start, 0), 0);

        var distances = new Dictionary<Node<T>, double>();
        foreach (var node in GetNodes())
        {
            distances[node] = double.PositiveInfinity;
        }
        distances[start] = 0;

        HashSet<Node<T>> visited = new HashSet<Node<T>>();

        while (queue.Count > 0)
        {
            var (currentNode, currentDist) = queue.Dequeue();
            if (currentDist > distances[currentNode]) { continue; }

            var unvisitedNeighbours = GetNeighbours(currentNode)
                .Where(item =>
                {
                    var (node, _) = item;
                    return !visited.Contains(node);
                })
                .ToList();

            foreach (var (neighbour, dist) in unvisitedNeighbours)
            {
                double new_dist = dist + currentDist;
                if (new_dist < distances[neighbour])
                {
                    distances[neighbour] = new_dist;
                    queue.Enqueue((neighbour, new_dist), new_dist);
                }
            }

            visited.Add(currentNode);
        }

        return distances;
    }

    public double Dijkstra(Node<T> start, Node<T> end)
    {
        if (!Nodes.Values.Contains(start))
        {
            throw new ArgumentException($"Start node not found: {start}");
        }
        if (!Nodes.Values.Contains(end))
        {
            throw new ArgumentException($"Node not found: {end}");
        }
        PriorityQueue<(Node<T>, double), double> queue = new PriorityQueue<(Node<T>, double), double>();
        queue.Enqueue((start, 0), 0);

        var distances = new Dictionary<Node<T>, double>();
        foreach (var node in GetNodes())
        {
            distances[node] = double.PositiveInfinity;
        }
        distances[start] = 0;

        HashSet<Node<T>> visited = new HashSet<Node<T>>();

        while (queue.Count > 0)
        {
            var (currentNode, currentDist) = queue.Dequeue();
            if (currentNode == end) { return distances[end]; }
            if (currentDist > distances[currentNode]) { continue; }

            var unvisitedNeighbours = GetNeighbours(currentNode)
                .Where(item =>
                {
                    var (node, _) = item;
                    return !visited.Contains(node);
                })
                .ToList();

            foreach (var (neighbour, dist) in unvisitedNeighbours)
            {
                double new_dist = dist + currentDist;
                if (new_dist < distances[neighbour])
                {
                    distances[neighbour] = new_dist;
                    queue.Enqueue((neighbour, new_dist), new_dist);
                }
            }

            visited.Add(currentNode);
        }

        return distances[end];
    }
}

class Program
{
    static Graph<char> BuildTestGraph()
    {
        var graph = new Graph<char>();
        foreach (char c in "ABCDEFGHI")
        {
            graph.AddNode(c);
        }

        graph.AddEdge('A', 'B', 12);
        graph.AddEdge('A', 'C', 8);
        graph.AddEdge('A', 'D', 11);
        graph.AddEdge('B', 'E', 19);
        graph.AddEdge('C', 'D', 6);
        graph.AddEdge('C', 'E', 7);
        graph.AddEdge('D', 'F', 8);
        graph.AddEdge('D', 'H', 2);
        graph.AddEdge('D', 'I', 15);
        graph.AddEdge('E', 'F', 5);
        graph.AddEdge('F', 'G', 6);
        graph.AddEdge('G', 'H', 1);
        graph.AddEdge('H', 'I', 11);

        return graph;
    }

    static void ReadInput(string filename)
    {
        using (StreamReader reader = File.OpenText(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }

    static void Main(string[] args)
    {
        var graph = BuildTestGraph();
        var start = graph.GetNode('A');
        var distances = graph.Dijkstra(start);
        foreach (var kvp in distances)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        try
        {
            Console.WriteLine(graph.Dijkstra(start, new Node<char>('G')));
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}