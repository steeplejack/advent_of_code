using System.Diagnostics;
using System.Net.Http.Headers;

namespace Day17;

/*
 * Today's task is to build a mini computer that can execute a program. It has three registers and
 * seven different operations.
 *
 * Part 1 is to work out the output of a particular program for a particular arrangement of register
 * values.
 *
 * Part 2 is to change the value of register A until the program outputs its own input. The idea that
 * worked for me was first to recognised that the input program is structured so that register A gets
 * bitshifted 3 places every iteration, and the output of each iteration only depends on the last 3
 * bits of register A. So you can check all 8 3bit values to match the last position of the program,
 * then shift them left 3 places, then add each of the 8 3bit values to the result until it matches the
 * last 2 positions of the program, and so on. Sometimes more than one value will provide a match, so
 * the solution space branches out a bit. This can be managed with a recursion.
 */


public struct Registers
{
    public long A { get; set; } = 0;
    public long B { get; set; } = 0;
    public long C { get; set; } = 0;

    public Registers(long a, long b, long c)
    {
        A = a;
        B = b;
        C = c;
    }

    public override string ToString()
    {
        return $"Register A: {A}\nRegister B: {B}\nRegister C: {C}";
    }
}

public class OpProgram
{
    private List<int> Program;
    private int InstructionPointer;

    public OpProgram()
    {
        Program = new List<int>();
        InstructionPointer = 0;
    }

    public OpProgram(string program)
    {
        Program = new List<int>();
        StringReader sr = new(program);
        char[] buffer = new char[4];
        foreach (char _ in "Program: ") sr.Read();
        while (sr.Read(buffer) > 0)
        {
            var (opcode, operand) = (int.Parse(buffer[0].ToString()), int.Parse(buffer[2].ToString()));
            Program.Add(opcode);
            Program.Add(operand);
        }

        SetInstructionPointer(0);
    }

    public bool SetInstructionPointer(int p)
    {
        if ((p & 1) == 1 || p < 0 || p >= Program.Count)
        {
            return false;
        }
        InstructionPointer = p;
        return true;
    }

    public bool AdvanceInstructionPointer()
    {
        InstructionPointer += 2;
        if (InstructionPointer >= Program.Count) return false;
        return true;
    }

    public bool InstructionPointerInBounds()
    {
        return InstructionPointer >= 0 && InstructionPointer < Program.Count;
    }

    public int GetOperator()
    {
        return Program[InstructionPointer];
    }

    public int GetOperand()
    {
        return Program[InstructionPointer + 1];
    }

    public List<int> GetProgram() => Program;

    public override string ToString()
    {
        string operations = String.Join(",", Program.Select(x => x.ToString()));
        return "Program: " + operations;
    }
}

class Runner
{
    private Registers _registers;
    private OpProgram _program;
    private List<int> _output = new List<int>();
    private Func<int, bool>[] _operations;

    public Runner(Registers registers, OpProgram program)
    {
        _registers = registers;
        _program = program;
        _operations = new Func<int, bool>[]
        {
            Adv, Bxl, Bst, Jnz, Bxc, Out, Bdv, Cdv,
        };
    }

    public Registers Registers => _registers;

    public void Run()
    {
        int opcode = _program.GetOperator();
        int operand = _program.GetOperand();
        bool advance = _operations[opcode].Invoke(operand);

        if (advance)
        {
            _program.AdvanceInstructionPointer();
        }

        if (_program.InstructionPointerInBounds())
        {
            Run();
        }
        else
        {
            string outputString = String.Join(",", _output.Select(x => x.ToString()));
            Console.WriteLine($"Output:  {outputString}");
        }
    }

   public long Combo(int operand)
    {
        return operand switch
        {
            >= 0 and <= 3 => operand,
            4 => _registers.A,
            5 => _registers.B,
            6 => _registers.C,
        };
    }
    public bool Adv(int operand)
    {
        _registers.A >>= (int)Combo(operand);
        return true;
    }

    public bool Bxl(int operand)
    {
        _registers.B ^= operand;
        return true;
    }

    public bool Bst(int operand)
    {
        _registers.B = Combo(operand) % 8;
        return true;
    }

    public bool Jnz(int operand)
    {
        if (_registers.A != 0)
        {
            _program.SetInstructionPointer((int)operand);
            return false;
        }

        return true;
    }

    public bool Bxc(int operand)
    {
        _registers.B ^= _registers.C;
        return true;
    }

    public bool Out(int operand)
    {
        _output.Add((int)(Combo(operand) % 8));
        return true;
    }

    private bool Bdv(int operand)
    {
        _registers.B = _registers.A >> (int)Combo(operand);
        return true;
    }

    private bool Cdv(int operand)
    {
        _registers.C = _registers.A >> (int)Combo(operand);
        return true;
    }
}

class Program
{
    static (Registers, OpProgram) ReadInput(string filename)
    {
        Registers registers = new();
        OpProgram program = new();

        using (StreamReader reader = File.OpenText(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("Register A"))
                {
                    registers.A = long.Parse(line.Split(":")[1].Trim());
                }

                else if (line.StartsWith("Register B"))
                {
                    registers.B = long.Parse(line.Split(":")[1].Trim());
                }

                else if (line.StartsWith("Register C"))
                {
                    registers.C = long.Parse(line.Split(":")[1].Trim());
                }

                else
                {
                    program = new(line);
                }
            }
        }

        return (registers, program);
    }

    static int RunIteration(long a)
    // Specific to the program in input.txt
    {
        var b = (a % 8) ^ 1;
        var c = (a >> (int)b);
        b = b ^ c ^ 4;
        return (int)(b % 8);
    }

    static List<int> RunProgram(long a)
    // Specific to the program in input.txt
    {
        List<int> output = new();
        while (a > 0)
        {
            output.Add(RunIteration(a));
            a >>= 3;
        }

        return output;
    }

    static bool Test(long a, List<int> program)
    {
        if (a == 0) { return false; }

        var result = RunProgram(a);
        return result.SequenceEqual(program);
    }

    static List<long> Quine(List<int> program)
    {
        int l = program.Count;
        Dictionary<int, List<long>> results = new();

        void Rec(long a, int n)
        {
            if (n > l) return;
            if (Test(a, program.GetRange(l-n, n)))
            {
                if (results.ContainsKey(n))
                {
                    results[n].Add(a);
                }
                else
                {
                    results.Add(n, [a]);
                }

                for (long i = 0; i < 8; i++)
                {
                    Rec((a << 3) + i, n + 1);
                }
            }
        }

        for (long i = 0; i < 8; i++)
        {
            Rec(i, 1);
        }
        return results[program.Count];
    }

    static void Main()
    {
        var (registers, program) = ReadInput("input.txt");
        string part1 = String.Join(",", RunProgram(registers.A));
        long part2 = Quine(program.GetProgram()).Min();
        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
    }
}