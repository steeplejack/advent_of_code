from dijkstra import Node, Graph, Edge, dijkstra, dijkstra_path

def read_input(filename):
    grid = []
    with open(filename, 'r') as f:
        for line in f:
            grid.append(list(line.strip()))
    return grid

def transpose(grid):
    return list(map(list, zip(*grid)))

def print_grid(grid):
    for row in grid:
        print(''.join(row))

def print_grid_with_highlight(grid, r, c):
    grid_copy = [row.copy() for row in grid.copy()]
    grid_copy[r][c] = '*'
    print_grid(grid_copy)

def grid_to_sets(grid):
    walls = set()
    spaces = set()
    for rownum in range(1, len(grid) - 1):
        row = grid[rownum]
        for colnum in range(1, len(row) - 1):
            square = row[colnum]
            if square == '#':
                walls.add((rownum, colnum))
            else:
                spaces.add((rownum, colnum))
    return walls, spaces

def grid_to_graph(grid):
    start = [0, 0]
    end = [0, 0]
    graph = Graph()

    for rownum in range(len(grid) - 1):
        for colnum in range(len(grid[rownum]) - 1):
            square = grid[rownum][colnum]
            if square == 'S':
                start = [rownum, colnum]
            if square == 'E':
                end = [rownum, colnum]

            current = (rownum, colnum)
            right = (rownum, colnum + 1)
            down = (rownum + 1, colnum)

            for node in [current, right, down]:
                if node not in graph.nodes:
                    graph.add_node(node)

            if square != '#':
                if grid[right[0]][right[1]] != '#':
                    graph.add_edge(current, right, 1)
                if grid[down[0]][down[1]] != '#':
                    graph.add_edge(current, down, 1)

    return graph, tuple(start), tuple(end)

def path_to_scores(path):
    return dict((node.value, dist) for (dist, node) in enumerate(path))

def picoseconds_saved(wall, scores):
    (i, j) = wall
    path_positions = [scores.get(neighbour, None) for neighbour in [(i, j + 1), (i, j - 1), (i + 1, j), (i - 1, j)] if neighbour in scores]
    if len(path_positions) < 2:
        return 0
    return max(path_positions) - min(path_positions) - 2


grid = read_input('input.txt')
walls, spaces = grid_to_sets(grid)
graph, start, end = grid_to_graph(grid)
start = graph.get_node(start)
end = graph.get_node(end)
dist, path = dijkstra_path(graph, start, end)
assert len(path) == 1
scores = path_to_scores(path[0])

results = {}

for wall in walls:
    saved = picoseconds_saved(wall, scores)
    if saved in results:
        results[saved] += 1
    else:
        results[saved] = 1

s = sum(count for saved, count in results.items() if saved >= 100)
print(f'Part 1: {s}')

