using System.Diagnostics;

namespace Day15;

using DirectionsList = List<Direction>;

public enum Direction
{
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
    None = 5,
}

public class Grid
{
    public char[,] grid { get; }
    public int rows { get; }
    public int cols { get; }
    public Grid(int rows, int columns)
    {
        this.rows = rows;
        this.cols = columns;
        char[,] grid = new char[rows, columns];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                grid[r, c] = '.';
            }
        }
        this.grid = grid;
    }

    public void Draw(List<WarehouseObject> objects)
    {
        foreach (var obj in objects)
        {
            var (r, c) = (obj.row, obj.col);
            this.grid[r, c] = Convert.ToChar(obj.ToString());
        }

        for (int r = 0; r < this.rows; r++)
        {
            for (int c = 0; c < this.cols; c++)
            {
                Console.Write(this.grid[r, c]);
                this.grid[r, c] = '.';
            }
            Console.WriteLine();
        }
    }
}

public class WarehouseObject(int row, int col, bool isStatic, bool isBlocked)
{
    public int row { get; set; } = row;
    public int col { get; set; }= col;
    public Direction dir { get; set; } = Direction.None;
    public bool isStatic = isStatic;
    public bool isBlocked = isBlocked;

    public (int, int) GetNextPosition()
    {
        return this.dir switch
        {
            Direction.Up => (row - 1, col),
            Direction.Down => (row + 1, col),
            Direction.Left => (row, col - 1),
            Direction.Right => (row, col + 1),
            Direction.None => (-1, -1),
        };
    }

    public void Move()
    {
        if (this.dir == Direction.None) return;
        (this.row, this.col) = GetNextPosition();
        this.dir = Direction.None;
    }
}

public class Wall(int row, int col) : WarehouseObject(row, col, true, true)
{
    public override string ToString()
    {
        return "#";
    }
}


public class Box(int row, int col) : WarehouseObject(row, col, false, false)
{
    public override string ToString()
    {
        return "O";
    }
}

public class Robot(int row, int col) : WarehouseObject(row, col, false, false)
{
    public override string ToString()
    {
        return "@";
    }
}

class Program
{
    static (Robot, List<WarehouseObject>, DirectionsList, Grid) ReadInput(string filename)
    {
        List<List<char>> grid = new();
        DirectionsList directions = new();
        using (StreamReader file = new(filename))
        {
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("#"))
                {
                    grid.Add(new(line));
                }
                else
                {
                    foreach (char c in line)
                    {
                        Direction d = c switch
                        {
                            '^' => Direction.Up,
                            '<' => Direction.Left,
                            'v' => Direction.Down,
                            '>' => Direction.Right,
                            _ => Direction.None,
                        };
                        directions.Add(d);
                    }
                }
            }
        }

        var (robot, objects, the_grid) = ConvertGrid(grid);

        return (robot, objects, directions, the_grid);
    }

    static (Robot, List<WarehouseObject>, Grid) ConvertGrid(List<List<char>> grid)
    {
        int height = grid.Count;
        int width = grid[0].Count;
        List<WarehouseObject> objects = new();
        Robot robot = new(height, width);

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c = grid[row][col];
                if (c == '#')
                {
                    objects.Add(new Wall(row, col));
                }
                else if (c == 'O')
                {
                    objects.Add(new Box(row, col));
                }
                else if (c == '@')
                {
                    robot = new(row, col);
                    objects.Add(robot);
                }
                else
                {
                    continue;
                }
            }
        }
        Grid the_grid = new(height, width);
        return (robot, objects, the_grid);
    }

    static void PrintDirection(Direction direction)
    {
        char c = direction switch
        {
            Direction.Up => '^',
            Direction.Down => 'v',
            Direction.Left => '<',
            Direction.Right => '>',
        };
        Console.WriteLine(c);
    }
    static void Main(string[] args)
    {
        var (robot, objects, directions, grid) = ReadInput(args[1]);
        grid.Draw(objects);
        foreach (Direction direction in directions)
        {
            // Console.WriteLine();
            // PrintDirection(direction);
            ApplyMovement(robot, direction);
            var applicableObjects = GetApplicableObjects(robot, objects);
            ResolveMovement(robot, applicableObjects);
            // grid.Draw(objects);
        }

        int result = BoxGpsSum(objects);
        Console.WriteLine($"Part 1: {result}");
    }

    static int BoxGpsSum(List<WarehouseObject> objects)
    {
        int s = 0;
        foreach (Box box in objects.OfType<Box>())
        {
            s += box.row * 100 + box.col;
        }

        return s;
    }

    static List<WarehouseObject> GetApplicableObjects(WarehouseObject curr, List<WarehouseObject> objects)
    {
        List<WarehouseObject> result = new();
        foreach (var obj in objects)
        {
            if (curr.dir == Direction.Left && curr.row == obj.row && obj.col < curr.col)
            {
                result.Add(obj);
            }
            else if (curr.dir == Direction.Right && curr.row == obj.row && obj.col > curr.col)
            {
                result.Add(obj);
            }
            else if (curr.dir == Direction.Down && curr.col == obj.col && obj.row > curr.row)
            {
                result.Add(obj);
            }
            else if (curr.dir == Direction.Up && curr.col == obj.col && obj.row < curr.row)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    static void ApplyMovement(WarehouseObject curr, Direction direction)
    {
        if (curr.isStatic) throw new Exception();
        curr.dir = direction;
    }

    static bool ResolveMovement(WarehouseObject curr, List<WarehouseObject> objects)
    {
        var (r, c) = curr.GetNextPosition();
        bool freeToMove = true;
        foreach (var obj in objects)
        {
            if (obj.row == r && obj.col == c)
            {
                if (obj.isStatic)
                {
                    return false;
                }
                else
                {
                    ApplyMovement(obj, curr.dir);
                    List<WarehouseObject> newObjects = GetApplicableObjects(obj, objects);
                    freeToMove = ResolveMovement(obj, newObjects);
                }
            }
        }

        if (freeToMove)
        {
            curr.Move();
            return true;
        }

        return false;
    }
}