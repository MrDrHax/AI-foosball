import matplotlib.pyplot as plt

toplot = []

with open('saves/b7f67842-3505-44bc-b6a0-a84307d77401/data.txt', 'r')as f:
    for l in f.readlines():
        toplot.append(float(l))

plt.plot(toplot)
plt.show()