
namespace Day16;

public enum Direction
{
    N, E, S, W,
}

public struct GridSquare : IEquatable<GridSquare>
{
    public int Row { get; }
    public int Col { get; }
    public Direction Direction { get; }
    public bool IsWall { get; }

    public GridSquare(int row, int col, Direction dir, bool isWall)
    {
        Row = row;
        Col = col;
        Direction = dir;
        IsWall = isWall;
    }

    public override string ToString()
    {
        return $"{Row}:{Col}:{Direction}:{IsWall}";
    }

    public override bool Equals(object? obj)
    {
        return obj is GridSquare other && Equals(other);
    }

    public bool Equals(GridSquare other)
    {
        return Row == other.Row && Col == other.Col && Direction == other.Direction && IsWall == other.IsWall;
    }

    public static bool operator ==(GridSquare? left, GridSquare? right) => left.Equals(right);
    public static bool operator !=(GridSquare? left, GridSquare? right) => !(left == right);

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Col, Direction, IsWall);
    }
}

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
        return Nodes.Values.ToArray();
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

        // if (!Edges.ContainsKey(tail_node))
        // {
        //     Edges[tail_node] = new Dictionary<Node<T>, double>();
        // }
        // Edges[tail_node][head_node] = distance;
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

    public (double, Dictionary<Node<T>, Node<T>>) Dijkstra(Node<T> start, Node<T> end)
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

        Dictionary<Node<T>, Node<T>> previous = new Dictionary<Node<T>, Node<T>>();

        while (queue.Count > 0)
        {
            var (currentNode, currentDist) = queue.Dequeue();
            if (currentNode == end) { return (distances[end], previous); }
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
                    previous[neighbour] = currentNode;
                    queue.Enqueue((neighbour, new_dist), new_dist);
                }
            }

            visited.Add(currentNode);
        }

        return (distances[end], previous);
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

    static (Graph<GridSquare>, (int, int), (int, int)) ReadInput(string filename)
    {
        Dictionary<(int, int), bool> gridSquareIsWall = new();
        Graph<GridSquare> graph = new();
        int max_row = 0;
        int max_col = 0;
        (int, int) start = (0, 0);
        (int, int) end = (0, 0);
        using (StreamReader reader = File.OpenText(filename))
        {
            string line;
            int curr_row = 0;
            while ((line = reader.ReadLine()) != null)
            {
                int curr_col = 0;
                foreach (char c in line.Trim())
                {
                    if (c == 'S')
                    {
                        start = (curr_row, curr_col);
                    }

                    if (c == 'E')
                    {
                        end = (curr_row, curr_col);
                    }
                    if (c != '#')
                    {
                        var north = new GridSquare(curr_row, curr_col, Direction.N, false);
                        var east = new GridSquare(curr_row, curr_col, Direction.E, false);
                        var south = new GridSquare(curr_row, curr_col, Direction.S, false);
                        var west = new GridSquare(curr_row, curr_col, Direction.W, false);
                        graph.AddNode(north);
                        graph.AddNode(east);
                        graph.AddNode(south);
                        graph.AddNode(west);
                        graph.AddEdge(north, west, 1000.0);
                        graph.AddEdge(north, east, 1000.0);
                        graph.AddEdge(east, north, 1000.0);
                        graph.AddEdge(east, south, 1000.0);
                        graph.AddEdge(south, west, 1000.0);
                        graph.AddEdge(south, east, 1000.0);
                        graph.AddEdge(west, north, 1000.0);
                        graph.AddEdge(west, south, 1000.0);
                    }

                    gridSquareIsWall.Add((curr_row, curr_col), c == '#');
                    curr_col++;
                    if (curr_col > max_col) { max_col = curr_col; }
                }
                curr_row++;
                if (curr_row > max_row) { max_row = curr_row; }
            }
        }


        for (int row = 0; row < max_row; row++)
        {
            for (int col = 0; col < max_col; col++)
            {
                if (gridSquareIsWall[(row, col)])
                {
                    continue;
                }

                var north = new GridSquare(row, col, Direction.N, false);
                var south = new GridSquare(row, col, Direction.S, false);
                var east = new GridSquare(row, col, Direction.E, false);
                var west = new GridSquare(row, col, Direction.W, false);

                if (row > 0)
                {
                    if (!gridSquareIsWall[(row - 1, col)])
                    {
                        var up = new GridSquare(row - 1, col, Direction.N, false);
                        graph.AddEdge(north, up, 1.0);
                    }
                }

                if (row < max_row - 1)
                {
                    if (!gridSquareIsWall[(row + 1, col)])
                    {
                        var down = new GridSquare(row + 1, col, Direction.S, false);
                        graph.AddEdge(south, down, 1.0);
                    }
                }

                if (col > 0)
                {
                    if (!gridSquareIsWall[(row, col - 1)])
                    {
                        var left = new GridSquare(row, col - 1, Direction.W, false);
                        graph.AddEdge(west, left, 1.0);
                    }
                }

                if (col < max_col - 1)
                {
                    if (!gridSquareIsWall[(row, col + 1)])
                    {
                        var right = new GridSquare(row, col + 1, Direction.E, false);
                        graph.AddEdge(east, right, 1.0);
                    }
                }
            }
        }

        return (graph, start, end);
    }

    static void Main(string[] args)
    {
        var (graph, start_pos, end_pos) = ReadInput(args[0]);
        // Console.WriteLine($"Start = {start_pos}, End = {end_pos}");

        GridSquare start = new(start_pos.Item1, start_pos.Item2, Direction.E, false);
        Node<GridSquare> start_node = graph.GetNode(start);

        double min_distance = double.MaxValue;
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            GridSquare end = new(end_pos.Item1, end_pos.Item2, direction, false);
            Node<GridSquare> end_node = graph.GetNode(end);
            var (distance, pathDict) = graph.Dijkstra(start_node, end_node);
            // Console.WriteLine($"End direction = {direction}. Distance = {distance}");
            if (distance < min_distance)
            {
                min_distance = distance;
            }

            List<Node<GridSquare>> path = new();
            var prev = end_node;
            while (prev != start_node)
            {
                path.Add(prev);
                prev = pathDict[prev];
            }
            path.Add(start_node);

            path.Reverse();

            // foreach (var node in path)
            // {
            //     Console.WriteLine($"({node.Value.Row}, {node.Value.Col}, {node.Value.Direction})");
            // }
        }
        Console.WriteLine($"Min distance = {min_distance}");
    }
}