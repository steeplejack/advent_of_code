import functools

class Problem:
    def __init__(self, target_string, component_string):
        self.target = int(target_string)
        self.components = tuple(int(x) for x in component_string.strip().split())

    def __repr__(self):
        return f'Problem({self.target}, {self.components})'

    def __len__(self):
        return len(self.components)

    def simple_solve(self):
        return sum(self.components) == self.target

    def solve(self):
        return solve(self.target, self.components[1:], self.components[0], '')

@functools.cache
def solve(target, components, accumulator, operations):
    # print(target, components, accumulator, operations)
    if len(components) == 0:
        return accumulator == target, accumulator, operations

    if accumulator > target:
        return False, accumulator, 'exceed'
    
    prod_arm = solve(target,
                 components[1:],
                 accumulator * components[0],
                 operations + '*')
    
    sum_arm = solve(target,
                 components[1:],
                 accumulator + components[0],
                 operations + '+')

    if prod_arm[0]:
        return prod_arm 

    if sum_arm[0]:
        return sum_arm

    return False, accumulator, 'fail'
        

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
        if prob.simple_solve():
            acc += prob.target
        else:
            soln = prob.solve()
            if soln[0]:
                acc += prob.target
    print(f'Part 1: {acc}')
    return acc

if __name__ == '__main__':
    result = main()
