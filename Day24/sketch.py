import re
import copy

def read_input(filename):
    wires = {}
    input_regex = r'^([xy]\d{2}): ([01])$'
    connection_regex = r'^(.*) (OR|XOR|AND) (.*) -> (.*)$'

    with open(filename, 'r') as fl:
        for line in fl:
            input_match = re.match(input_regex, line)
            if input_match:
                wire, value = input_match.groups()
                if wire in wires:
                    raise ValueError('Duplicate wire')
                wires[wire] = int(value)

            else:
                connection_match = re.match(connection_regex, line)
                if connection_match:
                    input1, logic_gate, input2, output = connection_match.groups()
                    if output in wires:
                        raise ValueError('Duplicate wire')
                    wires[output] = (input1, logic_gate, input2)

    return wires

def compute_gate(input1, input2, gate):
    if gate == 'AND':
        return input1 & input2
    elif gate == 'OR':
        return input1 | input2
    elif gate == 'XOR':
        return input1 ^ input2
    else:
        raise ValueError('Invalid gate')

def get_value(wire, wires):
    if wires[wire] in (0, 1):
        return wires[wire]

    input_key1, logic_gate, input_key2 = wires[wire]
    input1 = get_value(input_key1, wires)
    input2 = get_value(input_key2, wires)
    wires[wire] = compute_gate(input1, input2, logic_gate)
    return wires[wire]

def solve(filename):
    wires = read_input(filename)
    zkeys = filter(lambda x: x.startswith('z'), wires.keys())
    zkeys = sorted(zkeys, reverse=True)
    binary_string = ''.join(map(lambda x: str(get_value(x, wires)), zkeys))
    value = int(binary_string, 2)
    print(f'Value = {value}')

def add_xy(x, y):
    wires = read_input('input2.txt')
    for i in range(45):
        wires[f'x{i:02}'] = (x >> i) & 1
        wires[f'y{i:02}'] = (y >> i) & 1
    return get_z(wires), wires

def get_z(wires, inplace=False):
    if not inplace:
        w = copy.deepcopy(wires)
    z = 0
    for i in range(46):
        z += get_value(f'z{i:02}', w) << i
    return z

def adder(a, b, c):
    a = 1 if a > 0 else 0
    b = 1 if b > 0 else 0
    c = 1 if c > 0 else 0
    A = a ^ b
    sum = A ^ c
    B = A & c
    C = a & b
    carry = B | C
    return sum, carry

def to_dot(k, wires, id=''):
    a, g, c = wires[k]
    g += id
    print(f'{a} -> {g};\n{c} -> {g};\n{g} -> {k};')


def get_xyz(wires, inplace=False):
    if not inplace:
        w = copy.deepcopy(wires)
    x = 0
    y = 0
    z = 0
    for i in range(45):
        x |= get_value(f'x{i:02}', w) << i
        y |= get_value(f'y{i:02}', w) << i
        z |= get_value(f'z{i:02}', w) << i
    z |= get_value(f'z45', w) << 45
    return x, y, z

def part2_solution():
    """
    How do you do this other than by manual inspection of
    th4e graph of adder units?
    """
    # Show where the result goes wrong
    quit = False
    for i in range(46):
        x = 1 << i
        a, _ = add_xy(0, x)
        b, _ = add_xy(x, 0)
        c, _ = add_xy(x, x)
        if not a == x:
            print(f'a is wrong: i = {i}, x = {x}, a = {a}')
            quit = True
        if not b == x:
            print(f'b is wrong: i = {i}, x = {x}, b = {b}')
            quit = True
        if not c == (x + x):
            print(f'c is wrong: i = {i}, x = {x}, c = {c}')
            quit = True
    # Then look at the DOT graph around the i-values of the errors
    # to find the swaps.
    swaps = ['z09', 'nnf', 'nhs', 'z20', 'kqh', 'ddn', 'z34', 'wrc']
    print(','.join(sorted(swaps)))
