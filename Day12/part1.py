class Node:
    def __init__(self, value, row, column):
        self.value = value
        self.row = row
        self.column = column
        self.children = []
        self.visited = False
        self.maxcol = None

    def __repr__(self):
        return f"Node({self.value}, {self.row}, {self.column})"

    def set_maxcol(self, maxcol):
        self.maxcol = maxcol

    def add_child(self, node):
        self.children.append(node)

    def __hash__(self):
        return hash((self.value, self.row, self.column))

    def __eq__(self, other):
        return self.value == other.value

    def neighbours(self):
        if self.maxcol is None:
            raise Exception("maxcol not set")
        l = []
        if self.row > 0:
            l.append((self.row - 1, self.column))
        if self.row < self.maxcol:
            l.append((self.row + 1, self.column))
        if self.column > 0:
            l.append((self.row, self.column - 1))
        if self.column < self.maxcol:
            l.append((self.row, self.column + 1))
        return l

    def all_neighbours(self):
        return [(self.row - 1, self.column),
                (self.row + 1, self.column),
                (self.row, self.column - 1),
                (self.row, self.column + 1)]

def read_input(filename):
    grid = []
    maxcol = 0

    with open(filename, 'r') as f:
        for row, line in enumerate(f):
            line = line.rstrip()
            nodes = []
            for col, char in enumerate(line):
                nodes.append(Node(char, row, col))
                if col > maxcol:
                    maxcol = col
            grid.append(nodes)

    for row in grid:
        for node in row:
            node.set_maxcol(maxcol)
            for (r, c) in node.neighbours():
                neighbour = grid[r][c]
                if neighbour.value == node.value:
                    node.add_child(neighbour)
    
    return grid

def print_grid(grid):
    for row in grid:
        print(''.join(nd.value for nd in row))

def find_connected_component(node):
    queue = [node]
    seen = set()
    while len(queue) > 0:
        current = queue.pop()
        seen.add(current)
        for child in current.children:
            if child not in seen:
                queue.append(child)
    return sorted(seen, key = lambda n: (n.row, n.column))

def component_positions(component):
    positions = set()
    for node in component:
        positions.add((node.row, node.column))
    return positions

def count_perimeter(component):
    perimeter = 4 * len(component)
    positions = component_positions(component)
    for node in component:
        for (r, c) in node.all_neighbours():
            if (r, c) in positions:
                perimeter -= 1
    return perimeter

def find_connected_components(grid):
    components = []
    all_nodes = set()
    for row in grid:
        all_nodes.update(row)

    while len(all_nodes) > 0:
        node = all_nodes.pop()
        component = find_connected_component(node)
        components.append(component)
        all_nodes.difference_update(component)

    return components

grid = read_input("small_example.txt")
print_grid(grid)
cc = find_connected_components(grid)
s = 0
for c in cc:
    area = len(c)
    perimeter = count_perimeter(c)
    s += area * perimeter
    print(f'{c[0].value}: {area} * {perimeter} = {area * perimeter}')
print(s)
