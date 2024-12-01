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
    static void Main(string[] args)
    {
        var inputs = read_input_file(args[0]);
        int result = sum_of_sorted_differences(inputs.left, inputs.right);
        Console.WriteLine($"{result}");
    }
}