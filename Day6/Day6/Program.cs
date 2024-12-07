using System.Numerics;
using BlockadeList = System.Collections.Generic.HashSet<(int, int)>;

namespace Day6;

public enum Direction { Up, Down, Left, Right }

public class Guard
{
    public int row { get; set; }
    public int col { get; set; }
    public Direction direction { get; set; }

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

    static (BlockadeList, (int, int), int) ReadFile(string filename)
    {
        BlockadeList blockades = new();
        (int, int) startPos = (0, 0);
        int max = 0;
        using (StreamReader reader = new StreamReader(filename))
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
                        startPos = (rownum, colnum);
                    }
                }
                rownum++;
            }
        }
        return (blockades, startPos, max);
    }

    static bool FormsCycle((int, int) startPos, BlockadeList initialBlockades, (int, int) newBlockade, int max)
    {
        Guard guard = new Guard(startPos.Item1, startPos.Item2);
        BlockadeList blockades = new();
        foreach (var blockade in initialBlockades)
        {
            blockades.Add(blockade);
        }
        blockades.Add(newBlockade);

        HashSet<(int, int, Direction)> visited = new();
        visited.Add((guard.row, guard.col, guard.direction));

        int iter = 0;
        int maxIter = 1000000;
        while (!OutOfBounds((guard.row, guard.col), max) && iter < maxIter)
        {
            var next = guard.NextPos();

            if (blockades.Contains(next))
            {
                guard.Turn();
            }
            else
            {
                guard.Move();
            }

            var current = (guard.row, guard.col, guard.direction);

            if (visited.Contains(current))
            {
                return true;
            }

            visited.Add(current);

            iter++;
        }

        return false;
    }
    static HashSet<(int, int)> Part1(string[] args)
    {
        var (blockades, startPos, max) = ReadFile(args[1]);
        HashSet<(int, int)> trace = new();

        Guard guard = new Guard(startPos.Item1, startPos.Item2);
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
        Console.WriteLine($"Part 1: {trace.Count}");
        return trace;
    }

    static void Part2(string[] args, HashSet<(int, int)> path)
    {
        var (blockades, startPos, max) = ReadFile(args[1]);
        int cycleFormingBlockades = 0;
        Parallel.ForEach(path, (position) =>
        {
            var (row, col) = position;
            if (FormsCycle(startPos, blockades, (row, col), max))
            {
                System.Threading.Interlocked.Increment(ref cycleFormingBlockades);
            }
        });

        Console.WriteLine($"Part 2: {cycleFormingBlockades}");
    }

    static void Main(string[] args)
    {
        var path = Part1(args);
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        Part2(args, path);
        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    }
}