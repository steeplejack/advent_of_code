using System.Text.RegularExpressions;
struct Inputs
{
    public int[] left;
    public int[] right;

    public Inputs(int[] left, int[] right)
    {
        this.left = left;
        this.right = right;
    }
}

class Program
{
    static int sum_of_sorted_differences(in int[] left, in int[] right)
    {
        int[] x = (int[])left.Clone();
        int[] y = (int[])right.Clone();

        Array.Sort(x);
        Array.Sort(y);

        var zip = x.Zip(y, (a, b) => Math.Abs(b - a));

        var sum = 0;
        foreach (var diff in zip)
        {
            sum += diff;
        }

        return sum;
    }

    static Inputs read_input_file(in string filename)
    {
        var left = new List<int>();
        var right = new List<int>();

        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = Regex.Split(line, @"\s+");
                left.Add(int.Parse(values[0]));
                right.Add(int.Parse(values[1]));
            }
        }

        return new Inputs(left.ToArray(), right.ToArray());
    }

    static Dictionary<int, uint> count_occurences(int[] column)
    {
        var d = new Dictionary<int, uint>();
        foreach (var i in column)
        {
            if (d.ContainsKey(i))
            {
                d[i] += 1;
            }
            else
            {
                d[i] = 1;
            }
        }

        return d;
    }

    static long similarity_check(int[] left, int[] right)
    {
        var counts_left = count_occurences(left);
        var counts_right = count_occurences(right);
        long s = 0;
        foreach (var pair in counts_left)
        {
            var value = pair.Key;
            var left_count = pair.Value;
            uint right_count = 0;
            counts_right.TryGetValue(value, out right_count);
            s += value * left_count * right_count;
        }

        return s;
    }
    
    static void Main(string[] args)
    {
        var inputs = read_input_file(args[0]);
        int result = sum_of_sorted_differences(inputs.left, inputs.right);
        Console.WriteLine($"Part 1 = {result}");
        
        long result2 = similarity_check(inputs.left, inputs.right);
        Console.WriteLine($"Part 2 = {result2}");
    }
}