using System.Net.Mail;

namespace Day18;

public struct Coord(int row, int col)
{
    public int Row = row;
    public int Col = col;

    public override string ToString()
    {
        return $"Coord({Row}, {Col})";
    }

    public override bool Equals(object obj)
    {
        return (obj is Coord coord && coord.Row == Row && coord.Col == Col);
    }

    public bool Equals(Coord other)
    {
        return other.Row == Row && other.Col == Col;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Col);
    }
}

class Program
{
    static List<Coord> ReadInput(string filename)
    {
        List<Coord> coords = new();
        using (StreamReader reader = new(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var c = line.Trim().Split(',');
                Coord coord = new(int.Parse(c[0]), int.Parse(c[1]));
                coords.Add(coord);
            }
        }

        return coords;
    }

    static Graph<Coord> MakeGraph(List<Coord> coords)
    {
        int minRow = coords.Min(coord => coord.Row);
        int maxRow = coords.Max(coord => coord.Row);
        int minCol = coords.Min(coord => coord.Col);
        int maxCol = coords.Max(coord => coord.Col);

        var corrupted = coords.GetRange(0, 2917).ToHashSet();
        HashSet<Coord> created = new();

        Graph<Coord> graph = new();
        for (int i = minRow; i <= maxRow - 1; i++)
        {
            for (int j = minCol; j <= maxCol - 1; j++)
            {
                var coord = new Coord(i, j);
                var right = new Coord(i, j + 1);
                var down = new Coord(i + 1, j);

                if (!created.Contains(coord))
                {
                    graph.AddNode(coord);
                    created.Add(coord);
                }

                if (!created.Contains(right))
                {
                    graph.AddNode(right);
                    created.Add(right);
                }

                if (!created.Contains(down))
                {
                    graph.AddNode(down);
                    created.Add(down);
                }

                if (!corrupted.Contains(coord))
                {
                    if (!corrupted.Contains(right))
                    {
                        graph.AddEdge(coord, right, 1.0);
                        graph.AddEdge(right, coord, 1.0);
                    }

                    if (!corrupted.Contains(down))
                    {
                        graph.AddEdge(coord, down, 1.0);
                        graph.AddEdge(down, coord, 1.0);
                    }
                }
            }
        }

        for (int j = minCol; j < maxCol; j++)
        {
            var coord = new Coord(maxRow, j);
            var right = new Coord(maxRow, j + 1);
            if (!created.Contains(coord)) { graph.AddNode(coord); created.Add(coord); }
            if (!created.Contains(right)) { graph.AddNode(right); created.Add(right); }

            if (!corrupted.Contains(coord))
            {
                if (!corrupted.Contains(right))
                {
                    graph.AddEdge(coord, right, 1.0);
                    graph.AddEdge(right, coord, 1.0);
                }
            }
        }

        for (int i = minRow; i < maxRow; i++)
        {
            var coord = new Coord(i, maxCol);
            var down = new Coord(i + 1, maxCol);
            if (!created.Contains(coord)) { graph.AddNode(coord); created.Add(coord); }
            if (!created.Contains(down)) { graph.AddNode(down); created.Add(down); }

            if (!corrupted.Contains(coord))
            {
                if (!corrupted.Contains(down))
                {
                    graph.AddEdge(coord, down, 1.0);
                    graph.AddEdge(down, coord, 1.0);
                }
            }
        }

        return graph;
    }

    static void Main(string[] args)
    {
        List<Coord> input = ReadInput(args[1]);
        var graph = MakeGraph(input);
        var start = graph.GetNode(new Coord(0, 0));
        var end = graph.GetNode(new Coord(70, 70));
        var distance = graph.Dijkstra(start, end);
        Console.WriteLine($"Part 1: {distance}");


    }
}