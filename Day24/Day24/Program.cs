using System.Data;

namespace Day24;

using System.Text.RegularExpressions;

struct Wire
{
    public int? Value { get; set; }
    public string? Input1;
    public string? Input2;
    public string? Gate;

    public Wire(int value)
    {
        Value = value;
    }

    public Wire(string input1, string input2, string gate)
    {
        Input1 = input1;
        Input2 = input2;
        Gate = gate;
    }

    public override string ToString()
    {
        if (Value.HasValue)
        {
            return $"{Value}";
        }

        if (Input1 is not null && Input2 is not null && Gate is not null)
        {
            return $"{Input1} {Gate} {Input2}";
        }

        return "?@!?>@!";
    }

}

class Program
{
    static Dictionary<string, Wire> ReadInput(string filename)
    {
        Dictionary<string, Wire> wires = new();
        var lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            var inputMatch = Regex.Match(line, @"^([xy]\d{2}): ([01])$");
            var connectionMatch = Regex.Match(line, @"^(.*) (OR|XOR|AND) (.*) -> (.*)$");
            if (inputMatch.Success)
            {
                string wireName = inputMatch.Groups[1].Value;
                int value = int.Parse(inputMatch.Groups[2].Value);
                wires[wireName] = new Wire(value);
            }
            else if (connectionMatch.Success)
            {
                string inputWire1 = connectionMatch.Groups[1].Value;
                string logicGate = connectionMatch.Groups[2].Value;
                string inputWire2 = connectionMatch.Groups[3].Value;
                string outputWire = connectionMatch.Groups[4].Value;
                wires[outputWire] = new Wire(inputWire1, inputWire2, logicGate);
            }
        }

        return wires;
    }

    static int ComputeGate(int input1, int input2, string gate)
    {
        return gate switch
        {
            "OR" => input1 | input2,
            "XOR" => input1 ^ input2,
            "AND" => input1 & input2,
            _ => throw new ArgumentException("Gates must be AND, OR or XOR"),
        };
    }

    static int GetValue(string wireName, Dictionary<string, Wire> wires)
    {
        Wire wire = wires[wireName];
        if (wire.Value.HasValue) { return wire.Value.Value; }

        if (wire.Input1 is not null && wire.Input2 is not null && wire.Gate is not null)
        {
            string inputName1 = wire.Input1;
            string inputName2 = wire.Input2;
            string gate = wire.Gate;
            int input1 = GetValue(inputName1, wires);
            int input2 = GetValue(inputName2, wires);
            wire.Value = ComputeGate(input1, input2, gate);
            return wire.Value.Value;
        }

        else
        {
            throw new NoNullAllowedException("Wire has null inputs");
        }
    }

    static void Main(string[] args)
    {
        var wires = ReadInput(args[1]);

        var zkeys = wires.Keys.Where(k => k.StartsWith('z')).ToArray();

        long result = 0;
        foreach (var zkey in zkeys)
        {
            int value = GetValue(zkey, wires);
            if (value == 1)
            {
                int shift = int.Parse(zkey[1..]);
                result += 1L << shift;
            }
        }

        Console.WriteLine(result);
    }
}