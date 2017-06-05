from random import random
from time import sleep

if __name__ == '__main__':
    while True:
        with open('input.dat', 'w') as f:
            if random() > 0.5:
                f.write("1")
            else:
                f.write("0")
        sleep(1)
