class Problem:
    def __init__(self, target_string, component_string):
        self.target = int(target_string)
        self.components = tuple(int(x) for x in component_string.strip().split())

    def __repr__(self):
        return f'Problem({self.target}, {self.components})'

def solve(target, components):
    def inner(index, accumulator):
        if accumulator > target:
            return False
        
        if index == l:
            return accumulator == target

        return inner(index + 1, accumulator * components[index]) or \
            inner(index + 1, accumulator + components[index]) or \
            inner(index + 1, int(str(accumulator) + str_components[index]))

    l = len(components)
    str_components = [str(x) for x in components]
    return inner(1, components[0])

def read_input(filename):
    problems = []
    with open(filename) as fl:
        for line in fl:
            target, components = line.strip().split(':')
            problems.append(Problem(target, components))
    return problems

def main():
    problems = read_input('input.txt')

    acc = 0
    for prob in problems:
        if solve(prob.target, prob.components):
            acc += prob.target
    return acc

if __name__ == '__main__':
    result = main()
    print(f'Part2: {result}')
