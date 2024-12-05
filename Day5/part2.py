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


def pair_in_wrong_order(char_left, char_right, d):
    return char_left in d[char_right]

def bubble_sort(seq, d):
    for i in range(len(seq)-1):
        for j in range(i + 1, len(seq)):
            if pair_in_wrong_order(seq[i], seq[j], d):
                seq[i], seq[j] = seq[j], seq[i]
                return bubble_sort(seq, d)
    return seq

def main():
    d = construct_d('input.txt')
    seqs = [seq for seq in get_seqs('input.txt') if not check_seq(seq, d)]
    total = 0
    for seq in seqs:
        seq = bubble_sort(seq, d)
        if check_seq(seq, d):
            total += seq[len(seq) // 2]
    print(total)

if __name__ == '__main__':
    main()
