#!/usr/bin/env python3
"""
Script multiplataforma de empacotamento da extensão (substitui package.sh e package.ps1).

Uso:
    python package.py pub    -> compila, empacota (.vsix) e publica a extensão
    python package.py pack   -> compila e empacota (.vsix) a extensão
"""

import re
import shutil
import subprocess
import sys
from pathlib import Path


def run(comando: list[str]) -> None:
    """Executa um comando externo; encerra o script se ele falhar."""
    resultado = subprocess.run(comando, shell=(sys.platform == "win32"))
    if resultado.returncode != 0:
        sys.exit(resultado.returncode)


def remover_vsix() -> None:
    """Remove todos os arquivos .vsix da pasta atual (equivalente a rm/Remove-Item *.vsix)."""
    for arquivo in Path(".").glob("ptml-language-*.vsix"):
        arquivo.unlink()


def incrementar_versao(caminho: Path) -> None:
    """
    Incrementa em +1 o último número da versão em 'caminho' (ex.: scr/package.json).

    Regra: "major.minor.patch"
      - patch += 1
      - se patch chegar a 10: patch volta a 0 e minor += 1
      - se minor chegar a 10: minor volta a 0 e major += 1
    """
    if not caminho.is_file():
        print(f"[AVISO] Arquivo de versão não encontrado: {caminho}")
        return

    texto = caminho.read_text(encoding="utf-8")
    match = re.search(r'"version"\s*:\s*"(\d+)\.(\d+)\.(\d+)"', texto)
    if not match:
        print(f"[AVISO] Campo \"version\" não encontrado em: {caminho}")
        return

    major, minor, patch = (int(g) for g in match.groups())

    patch += 1
    if patch >= 10:
        patch = 0
        minor += 1
        if minor >= 10:
            minor = 0
            major += 1

    versao_antiga = match.group(0)
    versao_nova = f'"version": "{major}.{minor}.{patch}"'

    texto_novo = texto[: match.start()] + versao_nova + texto[match.end() :]
    caminho.write_text(texto_novo, encoding="utf-8")

    print(f"Versão atualizada em {caminho}: {versao_antiga} -> {versao_nova}")


def main() -> int:
    if len(sys.argv) != 2 or sys.argv[1] not in ("pub", "pack"):
        print(f"Uso: {sys.argv[0]} [pub|pack]")
        return 1

    comando = sys.argv[1]
    script_dir = Path(__file__).resolve().parent
    pasta_extensao = script_dir / "extension"

    if not pasta_extensao.is_dir():
        print(f"[ERRO] Pasta não encontrada: {pasta_extensao}")
        return 1

    diretorio_original = Path.cwd()
    try:
        import os
        os.chdir(pasta_extensao)

        npm = shutil.which("npm") or "npm"
        vsce = shutil.which("vsce") or "vsce"

        arquivo_versao = Path(".\\package.json")

        if comando == "pub":
            print("Executando publish...")
            remover_vsix()
            incrementar_versao(arquivo_versao)
            run([npm, "run", "compile"])
            run([vsce, "package"])
            run([vsce, "publish"])
        elif comando == "pack":
            print("Executando package...")
            remover_vsix()
            incrementar_versao(arquivo_versao)
            run([npm, "run", "compile"])
            run([vsce, "package"])
    finally:
        os.chdir(diretorio_original)

    return 0


if __name__ == "__main__":
    sys.exit(main())