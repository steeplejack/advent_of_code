from enum import Enum

class Direction(Enum):
    UP = 1
    DOWN = 2
    LEFT = 3
    RIGHT = 4
    NONE = 5

    def __str__(self):
        return {
            Direction.UP: '^',
            Direction.DOWN: 'v',
            Direction.LEFT: '<',
            Direction.RIGHT: '>',
            Direction.NONE: '!'
        }[self]


class Obj:
    def __init__(self, row, col, width, is_static):
        self.row = row
        self.col = col
        self.width = width
        self.is_static = is_static
        self.direction = None

    def occupies(self):
        return [(self.row, self.col + i) for i in range(self.width)]

    def moving_into(self):
        if self.direction == Direction.UP:
            return set([(self.row - 1, self.col + i) for i in range(self.width)])
        elif self.direction == Direction.DOWN:
            return set([(self.row + 1, self.col + i) for i in range(self.width)])
        elif self.direction == Direction.LEFT:
            return set([(self.row, self.col - 1)])
        elif self.direction == Direction.RIGHT:
            return set([(self.row, self.col + self.width)])
        return set([(self.row, self.col)])

    def move(self):
        if self.direction == Direction.UP:
            self.row -= 1
        elif self.direction == Direction.DOWN:
            self.row += 1
        elif self.direction == Direction.LEFT:
            self.col -= 1
        elif self.direction == Direction.RIGHT:
            self.col += 1
        self.direction = None


class Box(Obj):
    def __init__(self, row, col):
        super().__init__(row, col, width = 2, is_static = False)
    def __str__(self):
        return '[]'


class Robot(Obj):
    def __init__(self, row, col):
        super().__init__(row, col, width = 1, is_static = False)
    def __str__(self):
        return '@'


class Wall(Obj):
    def __init__(self, row, col):
        super().__init__(row, col, width = 2, is_static = True)
    def __str__(self):
        return '##'


class Grid:
    def __init__(self, height, width):
        self.height = height
        self.width = width
        self.grid = [['.' for _ in range(width)] for _ in range(height)]

    def draw(self, objects):
        for obj in objects:
            s = str(obj)
            for i in range(obj.width):
                self.grid[obj.row][obj.col + i] = s[i]
        print('\n'.join([''.join(row) for row in self.grid]))
        for row in self.grid:
            for i in range(self.width):
                row[i] = '.'


def read_input(filename):
    grid_height = 0
    grid_width = 0

    robot = None
    objects = []
    directions = []

    with open(filename) as fl:
        for line in fl:
            line = line.rstrip()
            if not line: continue
            if line.startswith('#'):
                row_length = len(line) * 2 # characters are double-width now
                if row_length > grid_width:
                    grid_width = row_length
                
                col = 0
                for char in line:
                    if char == '#':
                        objects.append(Wall(grid_height, col))
                    elif char == '@':
                        robot = Robot(grid_height, col)
                        objects.append(robot)
                    elif char == 'O':
                        objects.append(Box(grid_height, col))
                    col += 2

                grid_height += 1
            else:
                for char in line:
                    if char == '^':
                        directions.append(Direction.UP)
                    elif char == 'v':
                        directions.append(Direction.DOWN)
                    elif char == '<':
                        directions.append(Direction.LEFT)
                    elif char == '>':
                        directions.append(Direction.RIGHT)

    return robot, objects, grid_height, grid_width, directions


def raft_moving_into(raft, direction):
    """
    Helper function to find the 'wavefront' that the raft is moving into.
    The 'raft' is the robot plus all the boxes it is pushing.
    """
    occupied = set()
    wants = set()
    for obj in raft:
        obj.direction = direction
        occupied.update(obj.occupies())
        wants.update(obj.moving_into())
    return wants - occupied


def move_robot(robot, direction, objects):
    """
    Moves the robot and all its little boxes.
    """
    raft = [robot]
    resolved = False
    can_move = False
    while not resolved:
        moving_into = raft_moving_into(raft, direction)
        obstacles = []
        for obj in objects:
            if moving_into.intersection(obj.occupies()):
                obstacles.append(obj)
        if not obstacles:
            can_move = True
            resolved = True
        else:
            for obj in obstacles:
                if obj.is_static:
                    resolved = True
                    can_move = False
                else:
                    raft.append(obj)
    if can_move:
        for obj in raft:
            obj.move()


def score(objects):
    """
    Returns the score of the current state.
    """
    return sum([obj.row * 100 + obj.col for obj in objects if isinstance(obj, Box)])


robot, objects, height, width, directions = read_input('input.txt')
grid = Grid(height, width)
grid.draw(objects)
for direction in directions:
    move_robot(robot, direction, objects)
    # grid.draw(objects)
print(score(objects))
