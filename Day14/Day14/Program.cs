namespace Day14;

using System.Text.RegularExpressions;

public class Robot(int pos_x, int pos_y, int vel_x, int vel_y)
{
    internal int pos_x { get; set; } = pos_x;
    internal int pos_y { get; set; } = pos_y;
    internal int vel_x { get; set; } = vel_x;
    internal int vel_y { get; set; } = vel_y;
}

class Program
{
    static List<Robot> ReadInput(string filename)
    {
        List<Robot> robots = new();
        using (StreamReader reader = new(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Length == 0) continue;

                var match = Regex.Match(line, @"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");
                Robot robot = new(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value));
                robots.Add(robot);
            }
        }

        return robots;
    }

    static void MoveRobot(Robot robot, int seconds, int dim_x, int dim_y)
    {
        int new_x = (robot.pos_x + robot.vel_x * seconds) % dim_x;
        if (new_x < 0) new_x += dim_x;
        int new_y = (robot.pos_y + robot.vel_y * seconds) % dim_y;
        if (new_y < 0) new_y += dim_y;
        robot.pos_x = new_x;
        robot.pos_y = new_y;
    }

    static Dictionary<(int, int), int> MoveAllRobots(List<Robot> robots, int seconds, int max_x, int max_y)
    {
        Dictionary<(int, int), int> positionCounts = new();
        foreach (Robot robot in robots)
        {
            MoveRobot(robot, seconds, max_x, max_y);
            if (positionCounts.ContainsKey((robot.pos_x, robot.pos_y)))
            {
                positionCounts[(robot.pos_x, robot.pos_y)]++;
            }
            else
            {
                positionCounts[(robot.pos_x, robot.pos_y)] = 1;
            }
        }

        return positionCounts;
    }

    static int CountQuadrants(Dictionary<(int, int), int> positionCounts, int max_x, int max_y)
    {
        int topLeft = 0;
        int topRight = 0;
        int bottomLeft = 0;
        int bottomRight = 0;

        int middle_x = (int)max_x / 2;
        int middle_y = max_y / 2;

        foreach (KeyValuePair<(int, int), int> kvp in positionCounts)
        {
            var (x, y) = kvp.Key;
            var count = kvp.Value;
            if (x < middle_x && y < middle_y)
            {
                topLeft += count;
            }
            else if (x < middle_x && y > middle_y)
            {
                bottomLeft += count;
            }
            else if (x > middle_x && y < middle_y)
            {
                topRight += count;
            }
            else if (x > middle_x && y > middle_y)
            {
                bottomRight += count;
            }
        }
        return topLeft * topRight * bottomLeft * bottomRight;
    }

    static void Main(string[] args)
    {
        var robots = ReadInput(args[1]);
        Dictionary<(int, int), int> positionCounts = MoveAllRobots(robots, 100, 101, 103);
        int result = CountQuadrants(positionCounts, max_x: 101, max_y: 103);
        Console.WriteLine($"Part 1: {result}");

        MoveAllRobots(robots, 101 * 103 - 100, 101, 103); // Reset
        int rep = 0;
        int[] scores = new int[101 * 103];

        while (rep < 101 * 103)
        {
            var positions = MoveAllRobots(robots, 1, 101, 103);
            var score = CountQuadrants(positions, 101, 103);
            scores[rep] = score;

            rep++;
        }

        result = Array.IndexOf(scores, scores.Min()) + 1;
        Console.WriteLine($"Part 2: {result}");
    }
}