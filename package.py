#!/usr/bin/env python3
"""
Script multiplataforma de empacotamento da extensão (substitui package.sh e package.ps1).

Uso:
    python package.py pub    -> compila, empacota (.vsix) e publica a extensão
    python package.py pack   -> compila e empacota (.vsix) a extensão
"""

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
    for arquivo in Path(".").glob("*.vsix"):
        arquivo.unlink()


def main() -> int:
    if len(sys.argv) != 2 or sys.argv[1] not in ("pub", "pack"):
        print(f"Uso: {sys.argv[0]} [pub|pack]")
        return 1

    comando = sys.argv[1]
    script_dir = Path(__file__).resolve().parent
    pasta_extensao = script_dir / "syntax-highlighter"

    if not pasta_extensao.is_dir():
        print(f"[ERRO] Pasta não encontrada: {pasta_extensao}")
        return 1

    diretorio_original = Path.cwd()
    try:
        import os
        os.chdir(pasta_extensao)

        npm = shutil.which("npm") or "npm"
        vsce = shutil.which("vsce") or "vsce"

        if comando == "pub":
            print("Executando publish...")
            remover_vsix()
            run([npm, "run", "compile"])
            run([vsce, "package"])
            run([vsce, "publish"])
        elif comando == "pack":
            print("Executando package...")
            remover_vsix()
            run([npm, "run", "compile"])
            run([vsce, "package"])
    finally:
        os.chdir(diretorio_original)

    return 0


if __name__ == "__main__":
    sys.exit(main())