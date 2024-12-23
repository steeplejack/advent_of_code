class Computer:
    def __init__(self, name):
        self.name = name
        self.connections = []

    def __repr__(self):
        return f'Computer({self.name})'

    def add_connection(self, other_computer):
        self.connections.append(other_computer)

def read_input(filename):
    computers = {}
    with open(filename) as f:
        for line in f:
            computer_a, computer_b = line.rstrip().split('-')
            computer_a = computers.get(computer_a, Computer(computer_a))
            computer_b = computers.get(computer_b, Computer(computer_b))
            computer_a.add_connection(computer_b)
            computer_b.add_connection(computer_a)
            for computer in computer_a, computer_b:
                if not computer.name in computers:
                    computers[computer.name] = computer
    return computers


def perm3(lan):
    keys = list(lan.keys())
    n = len(keys)
    for i in range(n):
        ki = keys[i]
        ci = lan[ki]
        for j in range(i + 1, n):
            kj = keys[j]
            cj = lan[kj]
            for k in range(j + 1, n):
                kk = keys[k]
                ck = lan[kk]
                if ki[0] == 't' or kj[0] == 't' or kk[0] == 't':
                    if cj in ci.connections and ck in ci.connections and ck in cj.connections:
                        yield(sorted((ki, kj, kk)))

lan = read_input('input.txt')
g = perm3(lan)
i = 0
for sublist in sorted(g):
    print(','.join(sublist))
    i += 1
print(f"\nPart1: {i}")

