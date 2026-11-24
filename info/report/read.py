import csv

with open("Alvos.csv", "r", encoding="utf-8") as arquivo:
    leitor = csv.reader(arquivo)

    for linha in leitor:
        print("-------------------------------")
        for coluna in linha:
            print(coluna, end=" \n ")
        print("\n")