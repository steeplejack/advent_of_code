from collections import defaultdict

def construct_d(filename):
    d = defaultdict(set)
    with open(filename) as fl:
        lines = [l.strip() for l in fl.readlines() if '|' in l]
    for line in lines:
        a, b = (int(n) for n in line.split('|'))
        d[a].add(b)
    return d

def get_seqs(filename):
    with open(filename) as fl:
        lines = [l.strip() for l in fl.readlines() if l.strip() and '|' not in l]
    seqs = [[int(a) for a in b.split(',')] for b in lines]
    return seqs

def check_seq(seq, d):
    for i in range(len(seq)-1, 0, -1):
        char = seq[i]
        # print(f'i: {i}, char: {char}, seq[:i]: {seq[:i]}')
        for preceding in seq[:i]:
            if preceding in d[char]:
                return False
    return True

def main():
    d = construct_d('input.txt')
    seqs = get_seqs('input.txt')
    total = 0
    for seq in seqs:
        if check_seq(seq, d):
            total += seq[len(seq) // 2]
    print(total)

if __name__ == '__main__':
    main()
