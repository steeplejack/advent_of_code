using System.Data;
using System.Diagnostics;
using System.Numerics;

namespace Day22;

/*
 * In particular, each buyer's secret number evolves into the next secret number in the sequence via the following process:

   Calculate the result of multiplying the secret number by 64.
     Then, mix this result into the secret number. Finally, prune the secret number.
   Calculate the result of dividing the secret number by 32.
     Round the result down to the nearest integer. Then, mix this result into the secret number.
     Finally, prune the secret number.
   Calculate the result of multiplying the secret number by 2048.
     Then, mix this result into the secret number. Finally, prune the secret number.

   Each step of the above process involves mixing and pruning:

   To mix a value into the secret number, calculate the bitwise XOR of the given value and the secret number.
     Then, the secret number becomes the result of that operation.
     (If the secret number is 42 and you were to mix 15 into the secret number,
      the secret number would become 37.)
   To prune the secret number, calculate the value of the secret number modulo 16777216.
     Then, the secret number becomes the result of that operation.
     (If the secret number is 100000000 and you were to prune the secret number,
      the secret number would become 16113920.)
 */

using Prng32 = Prng<uint>;
using Prng64 = Prng<ulong>;
using Prng32Int = uint;
using Prng64Int = ulong;
public class Prng<T> where T: struct, IBinaryInteger<T>
{
    private T _seed;
    private readonly T _modulus;

    public Prng(T seed)
    {
        _seed = seed;
        _modulus = T.One << 24;
    }

    private void Mix(T val)
    {
        _seed ^= val;
    }

    private void Prune()
    {
        _seed %= _modulus;
    }

    public T Update()
    {
        checked
        {
            Mix(_seed << 6);
            Prune();
            Mix(_seed >> 5);
            Prune();
            Mix(_seed << 11);
            Prune();
            return _seed;
        }
    }

    public T Update(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Update();
        }
        return _seed;
    }

    public override string ToString()
    {
        return $"Prng({_seed})";
    }
}

class Program
{
    static List<int> ReadInput(string filename)
    {
        var input = new List<int>();
        using (StreamReader reader = new(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                input.Add(int.Parse(line));
            }
        }
        return input;
    }


    static List<int> GenerateSequence(int n, int length = 2000)
    {
        var current = n % 10;
        var gen = new Prng32((Prng32Int) n);
        var sequence = new List<int>{ current };

        while (sequence.Count < length)
        {
            sequence.Add((int) gen.Update() % 10);
        }

        return sequence;
    }

    static Dictionary<(int, int, int, int), long> GetAllChangeSequencesAndScores(List<int> input, int length)
    {
        var result = new Dictionary<(int, int, int, int), long>();
        foreach (var n in input)
        {
            var seen = new HashSet<(int, int, int, int)>();
            var sequence = GenerateSequence(n, length);
            for (int i = 0; i + 4 < sequence.Count; i++)
            {
                var diffs = (
                    sequence[i + 1] - sequence[i],
                    sequence[i + 2] - sequence[i + 1],
                    sequence[i + 3] - sequence[i + 2],
                    sequence[i + 4] - sequence[i + 3]);
                var score = sequence[i + 4];

                if (!seen.Contains(diffs))
                {
                    if (result.ContainsKey(diffs))
                    {
                        result[diffs] += score;
                    }
                    else
                    {
                        result[diffs] = score;
                    }
                    seen.Add(diffs);
                }
            }
        }
        return result;
    }

    static void Part1(List<int> input)
    {
        long s = 0;
        input.ForEach(n =>
        {
            var prng = new Prng32((Prng32Int) n);
            s += prng.Update(2000);
        });
        Console.WriteLine(s);
    }

    static void Part2(List<int> input)
    {
        var changes = GetAllChangeSequencesAndScores(input, 2001);
        Console.WriteLine(changes.Max(kvp => kvp.Value));
    }

    static void Main(string[] args)
    {
        var input = ReadInput(args[0]);
        Part1(input);
        Part2(input);
    }
}