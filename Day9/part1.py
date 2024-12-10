import sys

class Block:
    def __init__(self, label, reps, is_gap=False):
        self.label = label
        self.reps = reps
        self.is_gap = is_gap

    def __repr__(self):
        return f'({self.label}, {self.reps})'


def blocklist(s):
    blocks = []
    label = 0
    for i, char in enumerate(s):
        if i&1:
            block = Block('.', int(char), True)
        else:
            block = Block(label, int(char), False)
            label += 1
        blocks.append(block)
    return blocks

def first_empty(blocklist):
    for i, block in enumerate(blocklist):
        if block.is_gap and block.reps > 0:
            return i
    return -1

def last_full(blocklist):
    for i, block in enumerate(reversed(blocklist)):
        if not block.is_gap:
            if block.reps == 0: raise ValueError('Non-gaps should have a length>0')
            return len(blocklist) - i - 1
    return -1

def to_string(blocklist):
    s = ''
    for block in blocklist:
        s += str(block.label) * block.reps
    return s

def first_empty_with_size(blocklist, size):
    for i, block in enumerate(blocklist):
        if block.is_gap and block.reps >= size:
            return i
    return -1

def last_full_descending(blocklist, file_id):
    for i, block in enumerate(reversed(blocklist)):
        if not block.is_gap:
            if block.label <= file_id:
                return len(blocklist) - i - 1
    return -1

def move_contiguous(blocklist, file_id):
    l = last_full_descending(blocklist, file_id)
    f = first_empty_with_size(blocklist, blocklist[l].reps)
    if f != -1 and f < l:
        blocklist[f].reps -= blocklist[l].reps
        file_block = blocklist[l]
        blocklist[l] = Block('.', file_block.reps, True)
        blocklist = blocklist[:f] + [Block('.', 0, True)] + [file_block] + blocklist[f:]
        file_id = file_block.label
    return blocklist, file_id - 1


def move(blocklist):
    f = first_empty(blocklist)
    l = last_full(blocklist)
    lb = blocklist[l]
    if f == -1 or l == -1:
        return blocklist
    if f > l:
        print('Unexpected')
        return blocklist
    blocklist[f].reps -= 1
    blocklist[l].reps -= 1
    if blocklist[l].reps == 0:
        blocklist = blocklist[:l]
        while blocklist[-1].is_gap:
            blocklist.pop()
    if blocklist[f-1].label != lb.label:
        blocklist = blocklist[:f] + [Block('.', 0, True), Block(lb.label, 1, False)] + blocklist[f:]
    else:
        blocklist[f-1].reps += 1
    return blocklist

def checksum(blocklist):
    s = 0
    p = 0
    for block in blocklist:
        if block.is_gap:
            p += block.reps
        else:
            for _ in range(block.reps):
                s += p * block.label
                p += 1
    return s


# Part 1
s = open('input.txt').read().strip()
blocks = blocklist(s)
moves = 0
while first_empty(blocks) != -1:
    blocks = move(blocks)
    moves += 1
print(moves)
print(checksum(blocks))

# Part 2
s = open('input.txt').read().strip()
blocks = blocklist(s)
m = blocks[-1].label
while m >= 0:
    blocks, m = move_contiguous(blocks, m)
    # print(to_string(blocks))
print(checksum(blocks))
