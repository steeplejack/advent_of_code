import functools

def blink(n):
    if n == 0:
        return (1,)
    d = len(str(n))
    if d & 1:
        return (n * 2024,)
    return tuple(int(x) for x in divmod(n, 10**(d / 2)))

@functools.cache
def num(n, depth):
    res = blink(n)
    if depth == 0:
        return 1
    return sum(num(r, depth - 1) for r in res)

