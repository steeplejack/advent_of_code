using System.Text;

namespace Day4;

/*
 * Today's task is to do a wordsearch. There's a grid of letters, and we're looking for
 * as many 'XMAS' as we can find.
 */

class Program
{
    static List<(int, int)[]> MakeStencil(int size)
    // Words run in eight directions (up, down, left, right & 4 diagonals)
    // I'm making a list here of the coordinate offsets of words of length
    // `size` for each of the eight
    {
        var stencil = new List<(int, int)[]>();
        foreach (int i in new[] {-1, 0, 1})
        {
            foreach (int j in new[] {-1, 0, 1})
            {
                if (i == 0 && j == 0) continue;
                var coords = Enumerable.Range(0, size)
                    .Select(n => (n * i, n * j))
                    .ToArray();
                stencil.Add(coords);
            }
        }

        return stencil;
    }
    static char[,] LoadGrid(string filename, int wordLength = 4)
    // Here I load the grid with some padding round the edge so I don't need
    // to worry about going out of bounds when I run my stencil over it.
    {
        List<char[]> grid = new List<char[]>();

        var reader = new StreamReader(filename);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            grid.Add(line.Trim().ToCharArray());
        }

        int padding = wordLength - 1;
        char[,] newgrid = new char[grid.Count + 2 * padding, grid[0].Length + 2 * padding];
        for (int i = 0; i < grid.Count + 2*padding; i++)
        {
            for (int j = 0; j < grid[0].Length + 2*padding; j++)
            {
                newgrid[i,j] = '*';
            }
        }
        for (int i = 0; i < grid.Count; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                newgrid[i + padding, j + padding] = grid[i][j];
            }
        }
        
        return newgrid;
    }
    
    static void Main(string[] args)
    {
        int s = 0;
        string target = "XMAS";
        int wordLength = 4;
        int padding = wordLength - 1;
        char[,] grid = LoadGrid(args[1], wordLength);
        var stencil = MakeStencil(wordLength);
        
        // Scan over the grid (original grid, adjusted for padding)
        for (int i = padding; i < grid.GetLength(0) - padding; i++)
        {
            for (int j = padding; j < grid.GetLength(1) - padding; j++)
            {
                char gridChar = grid[i, j];
                // Whenever the grid letter matches the start of the target, look in all
                // 8 directions for a full match
                if (gridChar == target[0])
                {
                    foreach (var coords in stencil)
                    {
                        var wordBuilder = new StringBuilder();
                        foreach ((int x, int y) in coords)
                        {
                            wordBuilder.Append(grid[i + x, j + y]);
                        }
                        string word = wordBuilder.ToString();
                        if (word == target)
                        {
                            s += 1;
                        }
                    }
                }
            }
        }
        Console.WriteLine(s);
    }
}