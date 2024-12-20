from dijkstra import Node, Graph, Edge, dijkstra, dijkstra_path
from time import time_ns

def read_input(filename):
    grid = []
    with open(filename, 'r') as f:
        for line in f:
            grid.append(list(line.strip()))
    return grid

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

def manhattan_distance(v1, v2):
    return abs(v1[0] - v2[0]) + abs(v1[1] - v2[1])

def abs(x):
    return x if x > 0 else -x

def number_of_cheats_available(cheat_radius, saving_cutoff, scores):
    k = 0

    offsets = list(set((i * d[0], j * d[1])
        for i in range(cheat_radius + 1)
        for j in range(cheat_radius - i + 1)
        for d in [(-1, -1), (-1, 1), (1, -1), (1, 1)]))

    for v in scores:
        for offset in offsets:
            v2 = (v[0] + offset[0], v[1] + offset[1])
            if v2 in scores:
                manhattan = manhattan_distance(v, v2)
                pathdist = scores[v2] - scores[v]
                saving = pathdist - manhattan
                if saving >= saving_cutoff:
                    k += 1
    return k


grid = read_input('input.txt')
graph, start, end = grid_to_graph(grid)
start = graph.get_node(start)
end = graph.get_node(end)

_, path = dijkstra_path(graph, start, end)
assert len(path) == 1
scores = path_to_scores(path[0])

start_ex = time_ns()
k = number_of_cheats_available(2, 100, scores)
end_ex = time_ns()
print(f'Part 1: {k}')
print(f'Execution time: {(end_ex - start_ex) / 1_000_000}ms')

start_ex = time_ns()
k = number_of_cheats_available(20, 100, scores)
end_ex = time_ns()
print(f'Part 2: {k}')
print(f'Execution time: {(end_ex - start_ex) / 1_000_000}ms')

