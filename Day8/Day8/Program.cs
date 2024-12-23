﻿using System.Collections.Generic;
namespace Day8;

/*
 * What's going on today? There's a map of radio masts represented by a letter or number ID.
 * The task is to find all pairs of masts with the same ID and find two 'antinodes', which
 * are 2 points a set distance from the masts along the line between them (if M1 and M2 are the
 * masts, and D is the distance between them, then the antinodes are going to be at the point D
 * away from M1 in the opposite direction to M2, and vice versa). The solution os the number of
 * antinodes you find that are within the bounds of the grid.
 *
 * Part 2 extends part 1 by asking for more antinodes. Now they are found at all points along
 * the line from M1 to M2 at D-sized intervals, including at the positions of M1 and M2.
 */

internal class Vector(int x, int y)
{
    internal int x { get; } = x;
    internal int y { get; } = y;
}

internal class Point(int x, int y) : IEquatable<Point>
{
    internal int x { get; } = x;
    internal int y { get; } = y;

    public override bool Equals(object obj)
    {
        return Equals(obj as Point);
    }

    public bool Equals(Point p)
    {
        if (p == null) return false;
        return x == p.x && y == p.y;
    }

    public override int GetHashCode()
    {
        int xId = x.GetHashCode();
        int yId = y.GetHashCode();
        return x ^ y;
    }

    internal Vector Displacement(Point other)
    {
        return new Vector(other.x - x, other.y - y);
    }

    internal Point Add(Vector displacement)
    {
        return new Point(x + displacement.x, y + displacement.y);
    }

    internal Point Sub(Vector displacement)
    {
        return new Point(x - displacement.x, y - displacement.y);
    }

    internal bool InBounds(int max)
    {
        return x <= max && y <= max && x >= 0 && y >= 0;
    }

    internal Point Copy()
    {
        return new Point(x, y);
    }
}

class Program
{
    static int Part2(Dictionary<char, List<Point>> points, int max)
    {
        HashSet<Point> nodeSet = new();
        foreach (var kvp in points)
        {
            var pairs = Pairs(kvp.Value);
            foreach (var (p1, p2) in pairs)
            {
                var displacement = p1.Displacement(p2);
                Point node1 = p1.Copy();
                while (node1.InBounds(max))
                {
                    nodeSet.Add(node1);
                    node1 = node1.Sub(displacement);
                }

                Point node2 = p2.Copy();
                while (node2.InBounds(max))
                {
                    nodeSet.Add(node2);
                    node2 = node2.Add(displacement);
                }
            }
        }

        return nodeSet.Count;
    }

    static int Part1(Dictionary<char, List<Point>> points, int max)
    {
        HashSet<Point> nodeSet = new HashSet<Point>();
        foreach (var kvp in points)
        {
            var pairs = Pairs(kvp.Value);
            foreach (var (p1, p2) in pairs)
            {
                var displacement = p1.Displacement(p2);
                Point node1 = p1.Sub(displacement);
                Point node2 = p2.Add(displacement);
                if (node1.InBounds(max)) nodeSet.Add(node1);
                if (node2.InBounds(max)) nodeSet.Add(node2);
            }
        }

        return nodeSet.Count;
    }

    static void Main(string[] args)
    {
        var (points, max) = ReadFile(args[1]);
        Console.WriteLine($"Part 1: {Part1(points, max)} nodes");
        Console.WriteLine($"Part 2: {Part2(points, max)} nodes");
    }

    static List<(Point, Point)> Pairs(List<Point> points)
    {
        int length = points.Count;
        List<(Point, Point)> pairs = new();
        for (int i = 0; i < length - 1; i++)
        {
            for (int j = i + 1; j < length; j++)
            {
                pairs.Add((points[i], points[j]));
            }
        }

        return pairs;
    }

    static (Dictionary<char, List<Point>>, int) ReadFile(string filename)
    {
        Dictionary<char, List<Point>> points = new();
        int max = 0;
        using (StreamReader reader = new(filename))
        {
            string line;
            int row = 0;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                int col = 0;
                foreach (char c in line)
                {
                    if (c != '.')
                    {
                        if (!points.ContainsKey(c))
                        {
                            points[c] = new List<Point>();
                        }
                        points[c].Add(new Point(row, col));
                    }

                    if (col > max)
                    {
                        max = col;
                    }
                    col++;
                }

                row++;
            }
        }
        return (points, max);
    }
}