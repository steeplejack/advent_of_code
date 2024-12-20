from queue import PriorityQueue

class Node:
    def __init__(self, value):
        self.value = value

    def __repr__(self):
        return f'Node({self.value})'

    def __eq__(self, other):
        return self.value == other.value

    def __hash__(self):
        return hash(self.value)

    def __lt__(self, other):
        return self.value < other.value

class Edge:
    def __init__(self, head, tail, distance):
        self.head = head
        self.tail = tail
        self.distance = distance
    
    def __repr__(self):
        return f'Edge({self.head}---{self.distance}---{self.tail})'

class Graph:
    def __init__(self):
        self.nodes = {}
        self.edges = {}

    def add_node(self, value):
        if value in self.nodes:
            raise ValueError("Node already in graph")
        node = Node(value)
        self.nodes[value] = node

    def get_nodes(self):
        return sorted(self.nodes.values(), key=lambda x: x.value)

    def get_node(self, value):
        if value not in self.nodes:
            raise ValueError("Node not in graph")
        return self.nodes[value]

    def add_edge(self, head, tail, distance):
        head_node = self.get_node(head)
        tail_node = self.get_node(tail)
        if head_node in self.edges:
            self.edges[head_node][tail_node] = distance
        else:
            self.edges[head_node] = {tail_node: distance}
        if tail_node in self.edges:
            self.edges[tail_node][head_node] = distance
        else:
            self.edges[tail_node] = {head_node: distance}

    def get_edge(self, head, tail):
        head_node = self.get_node(head)
        tail_node = self.get_node(tail)
        if head_node in self.edges:
            if tail_node in self.edges[head_node]:
                dist = self.edges[head_node][tail_node]
                return Edge(head_node, tail_node, dist)
        raise ValueError("Edge not in graph")

    def get_neighbours(self, node):
        neighbours = []
        for neighbour, distance in self.edges[node].items():
            neighbours.append((distance, neighbour))
        return neighbours

def dijkstra(graph, start, end=None):
    distances = {node: float('inf') for node in graph.get_nodes()}
    visited = set()
    distances[start] = 0
    queue = PriorityQueue()
    queue.put((0, start))

    while not queue.empty():
        current_dist, current_node = queue.get()

        if end is not None and end == current_node:
            return distances[end]

        if current_dist > distances[current_node]:
            continue

        unvisited_neighbours = [(dist, node) for (dist, node) in graph.get_neighbours(current_node) if node not in visited]
        
        for dist, neighbour in unvisited_neighbours:
            new_dist = current_dist + dist
            if new_dist < distances[neighbour]:
                distances[neighbour] = new_dist
                item = (new_dist, neighbour)
                assert isinstance(item, tuple)
                assert isinstance(item[0], int)
                assert isinstance(item[1], Node)
                queue.put(item)
        visited.add(current_node)

    return distances

def dijkstra_path(graph, start, end=None):
    distances = {node: float('inf') for node in graph.get_nodes()}
    distances[start] = 0
    paths = {node: [] for node in graph.get_nodes()}
    paths[start] = [[start]]
    queue = PriorityQueue()
    queue.put((0, start))

    while not queue.empty():
        current_dist, current_node = queue.get()

        if end is not None and end == current_node:
            return distances[end], paths[end]

        if current_dist > distances[current_node]:
            continue

        for dist, neighbour in graph.get_neighbours(current_node):
            new_dist = current_dist + dist
            if new_dist < distances[neighbour]:
                distances[neighbour] = new_dist
                paths[neighbour] = [path + [neighbour] for path in paths[current_node]]
                queue.put((new_dist, neighbour))
            elif new_dist == distances[neighbour]:
                paths[neighbour].extend([path + [neighbour] for path in paths[current_node]])

    return distances, paths

