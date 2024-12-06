using System.Numerics;

namespace Day6;

enum Direction { Up, Down, Left, Right }

public class Guard
{
    public int row { get; set; }
    public int col { get; set; }
    Direction direction { get; set; }
    
    public Guard(int row, int col)
    {
        this.row = row;
        this.col = col;
        this.direction = Direction.Up;
    }

    public (int, int) NextPos()
    {
        return direction switch
        {
            Direction.Up => (row - 1, col),
            Direction.Down => (row + 1, col),
            Direction.Left => (row, col - 1),
            Direction.Right => (row, col + 1),
        };
    }

    public void Move()
    {
        (this.row, this.col) = NextPos();
    }

    public void Turn()
    {
        this.direction = direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
        };
    }
}

class Program
{
    static bool OutOfBounds((int, int) point, int max)
    {
        var (x, y) = (point.Item1, point.Item2);
        return (x < 0 || y < 0 || x > max || y > max);
    }
    static void Main(string[] args)
    {
        List<(int, int)> blockades = new();
        HashSet<(int, int)> trace = new();
        (int, int)? current = null;
        int max = 0;
        using (StreamReader reader = new StreamReader(args[1]))
        {
            string line;
            int rownum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                for (int colnum = 0; colnum < line.Length; colnum++)
                {
                    if (colnum > max) { max = colnum; }
                    if (line[colnum] == '#')
                    {
                        blockades.Add((rownum, colnum));
                    }
                    else if (line[colnum] == '^')
                    {
                        current = (rownum, colnum);
                        trace.Add((rownum, colnum));
                    }
                }
                rownum++;
            }
        }

        Console.WriteLine($"Starting point: {current}");
        Console.WriteLine($"Max: {max}");
        Guard guard = new Guard(current.Value.Item1, current.Value.Item2);
        while (!OutOfBounds((guard.row, guard.col), max))
        {
            trace.Add((guard.row, guard.col));
            var next = guard.NextPos();
            if (blockades.Contains(next))
            {
                guard.Turn();
            }
            else
            {
                guard.Move();
            }
        }
        // foreach ((int rownum, int colnum) in trace)
        // {
        //     Console.WriteLine($"Trace: ({rownum}, {colnum})");
        // }
        Console.WriteLine($"Steps: {trace.Count}");
    }
}