#!/usr/bin/env python3
"""
Script de build multiplataforma (substitui build.sh e build.ps1).

Faz o mesmo que os scripts originais:
  1. Garante que a pasta 'build' exista.
  2. Se o dotnet NÃO estiver instalado, apenas adiciona a pasta 'build' ao PATH do usuário.
  3. Se o dotnet estiver instalado, compila e publica o projeto em 'src' para dentro
     da pasta 'build' e então adiciona essa pasta ao PATH do usuário.
  4. Executa 'ptml --help --version' e 'ptml run "index.ptml" --window'.
"""

import os
import platform
import re
import shutil
import subprocess
import sys
from pathlib import Path

# --------------------------------------------------------------------------
# Cores para output no terminal (funcionam em Linux/macOS e Windows 10+)
# --------------------------------------------------------------------------
class Cores:
    VERMELHO = "\033[31m"
    VERDE = "\033[32m"
    AMARELO = "\033[33m"
    CIANO = "\033[36m"
    BRANCO = "\033[37m"
    RESET = "\033[0m"


def log(mensagem: str, cor: str = "") -> None:
    print(f"{cor}{mensagem}{Cores.RESET}")


# --------------------------------------------------------------------------
# Funções auxiliares
# --------------------------------------------------------------------------
def test_dotnet_installed() -> bool:
    """Equivalente a Test-DotNetInstalled / test_dotnet_installed."""
    return shutil.which("dotnet") is not None


def add_folder_to_user_path(target_folder: Path) -> bool:
    """
    Adiciona a pasta informada ao PATH do usuário de forma persistente.
    - No Windows: grava no registro (variável de ambiente do usuário).
    - No Linux/macOS: grava no arquivo de configuração do shell (~/.bashrc ou ~/.bash_profile).
    """
    try:
        absolute_caminho = target_folder.resolve(strict=True)
    except FileNotFoundError:
        log(f"[ERRO] A pasta especificada não existe: {target_folder}", Cores.VERMELHO)
        return False

    # Navega até a pasta (conforme os scripts originais)
    os.chdir(absolute_caminho)
    log(f"[INFO] Navegou para a pasta: {absolute_caminho}", Cores.CIANO)

    sistema = platform.system()

    if sistema == "Windows":
        return _add_to_windows_path(absolute_caminho)
    else:
        return _add_to_unix_path(absolute_caminho)


def _add_to_windows_path(absolute_caminho: Path) -> bool:
    import winreg  # disponível apenas no Windows

    try:
        # Abre a chave do PATH de usuário no registro do Windows
        chave = winreg.OpenKey(winreg.HKEY_CURRENT_USER, "Environment", 0, winreg.KEY_READ | winreg.KEY_WRITE)
        try:
            current_path, _ = winreg.QueryValueEx(chave, "Path")
        except FileNotFoundError:
            current_path = ""

        path_list = [p.strip() for p in current_path.split(";") if p.strip()]

        if str(absolute_caminho) in path_list:
            log("[AVISO] Esta pasta ja esta registrada no seu PATH do Windows.", Cores.AMARELO)
            winreg.CloseKey(chave)
            return True

        novo_path = f"{current_path};{absolute_caminho}"
        novo_path = re.sub(r";+", ";", novo_path)

        winreg.SetValueEx(chave, "Path", 0, winreg.REG_EXPAND_SZ, novo_path)
        winreg.CloseKey(chave)

        log("[SUCESSO] A pasta foi adicionada ao PATH do Windows com sucesso!", Cores.VERDE)
        log("Nota: Voce precisará reiniciar o terminal/VS Code para que as alterações façam efeito.", Cores.BRANCO)
        return True
    except OSError as erro:
        log(f"[ERRO] Falha ao atualizar o PATH: {erro}", Cores.VERMELHO)
        return False


def _add_to_unix_path(absolute_caminho: Path) -> bool:
    home = Path.home()
    bash_profile = home / (".bash_profile" if platform.system() == "Darwin" else ".bashrc")

    path_atual = os.environ.get("PATH", "")
    if f":{absolute_caminho}:" in f":{path_atual}:":
        log("[AVISO] Esta pasta já está registrada no seu PATH.", Cores.AMARELO)
        return True

    try:
        with open(bash_profile, "a", encoding="utf-8") as arquivo:
            arquivo.write("\n")
            arquivo.write("# Adicionado automaticamente pelo script de build\n")
            arquivo.write(f'export PATH="$PATH:{absolute_caminho}"\n')

        log("[SUCESSO] A pasta foi adicionada ao PATH com sucesso!", Cores.VERDE)
        log(
            f"Nota: Você precisará reiniciar o terminal ou rodar 'source {bash_profile}' "
            "para que as alterações façam efeito.",
            Cores.BRANCO,
        )
        return True
    except OSError:
        log("[ERRO] Falha ao atualizar o PATH.", Cores.VERMELHO)
        return False


def run(comando: list[str]) -> int:
    """Executa um comando externo e retorna o código de saída."""
    resultado = subprocess.run(comando)
    return resultado.returncode


# --------------------------------------------------------------------------
# Lógica principal
# --------------------------------------------------------------------------
def main() -> int:
    script_dir = Path(__file__).resolve().parent
    build_path = script_dir / "build"
    src_path = script_dir / "src"

    # Cria a pasta build caso ela não exista ainda para evitar erros de navegação inicial
    build_path.mkdir(parents=True, exist_ok=True)
    os.chdir(build_path)

    if not test_dotnet_installed():
        if not add_folder_to_user_path(build_path):
            return 1
    else:
        if not src_path.is_dir():
            log("[ERRO] Pasta 'src' não encontrada.", Cores.VERMELHO)
            return 1
        os.chdir(src_path)

        # RID de publicação: win-x64 no Windows, linux-x64 no Linux, osx-x64 no macOS
        rid_por_sistema = {
            "Windows": "win-x64",
            "Linux": "linux-x64",
            "Darwin": "osx-x64",
        }
        rid = rid_por_sistema.get(platform.system(), "win-x64")

        if run(["dotnet", "build"]) != 0:
            return 1
        if run([
            "dotnet", "publish",
            "-c", "Release",
            "-r", rid,
            "--self-contained", "true",
            "-o", str(build_path),
        ]) != 0:
            return 1

        os.chdir(build_path)
        if not add_folder_to_user_path(build_path):
            return 1

    os.chdir(script_dir)

    # Atualiza o PATH apenas para a sessão atual, para rodar os comandos abaixo
    os.environ["PATH"] = f"{os.environ.get('PATH', '')}{os.pathsep}{build_path}"

    run(["ptml", "--help", "--version"])
    run(["ptml", "run", "index.ptml", "--window"])

    return 0


if __name__ == "__main__":
    sys.exit(main())