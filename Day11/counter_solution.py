import functools
from collections import Counter

def digits(n):
    return len(str(n))

@functools.cache
def blink(n):
    if n == 0:
        return (1,)
    d = digits(n)
    if d & 1:
        return (n * 2024,)
    return tuple(int(x) for x in divmod(n, 10**(d / 2)))

def update_counter(cr):
    items = list(cr.items())
    for key, val in items:
        cr[key] -= val
        for result in blink(key):
            cr[result] += val


n = [int(x) for x in '814 1183689 0 1 766231 4091 93836 46'.split()]
c = Counter(n)
for _ in range(25):
    update_counter(c)
print(sum(c.values()))

for _ in range(50):
    update_counter(c)
print(sum(c.values()))
