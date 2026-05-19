let size_str(texto: string) =
    let rec loop(indice) =
        if indice >= texto.Length then
            0
        else
            1 + loop (indice + 1)
    loop(0)

let word = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"

printf "%d\n" (size_str(word))