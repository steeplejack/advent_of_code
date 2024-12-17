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
 * Part 2 is to change the value of register A until the program outputs its own input.
 */


public struct Registers
{
    public int A { get; set; } = 0;
    public int B { get; set; } = 0;
    public int C { get; set; } = 0;

    public Registers(int a, int b, int c)
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

    public override string ToString()
    {
        string operations = String.Join(",", Program.Select(x => x.ToString()));
        List<char> pointer = new();
        for (int i = 0; i < Program.Count * 2; i++)
        {
            if (i == InstructionPointer * 2)
            {
                pointer.Add('^');
                break;
            }
            else
            {
                pointer.Add(' ');
            }
        }

        return "*\n" + operations + '\n' + new String(pointer.ToArray()) + "\n*";
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
            Console.WriteLine($"Output: {outputString}";
        }
    }

   public int Combo(int operand)
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
        int opvalue = Combo(operand);
        int divisor = 1 << opvalue;
        _registers.A /= divisor;
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
            _program.SetInstructionPointer(operand);
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
        _output.Add(Combo(operand) % 8);
        return true;
    }

    private bool Bdv(int operand)
    {
        int opvalue = Combo(operand);
        int divisor = 1 << opvalue;
        _registers.B = _registers.A / divisor;
        return true;
    }

    private bool Cdv(int operand)
    {
        int opvalue = Combo(operand);
        int divisor = 1 << opvalue;
        _registers.C = _registers.A / divisor;
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
                    registers.A = int.Parse(line.Split(":")[1].Trim());
                }

                else if (line.StartsWith("Register B"))
                {
                    registers.B = int.Parse(line.Split(":")[1].Trim());
                }

                else if (line.StartsWith("Register C"))
                {
                    registers.C = int.Parse(line.Split(":")[1].Trim());
                }

                else
                {
                    program = new(line);
                }
            }
        }

        return (registers, program);
    }

    static void Main(string[] args)
    {
        var (registers, program) = ReadInput(args[1]);
        Console.WriteLine($"Inital Registers:\n{registers}");
        Console.WriteLine($"Program: {program}");
        Runner runner = new(registers, program);
        runner.Run();
        Console.WriteLine($"Final Registers:\n{runner.Registers}");
    }
}