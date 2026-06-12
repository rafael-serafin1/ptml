#!/usr/bin/env bash

# Suposto funcionar em ambientes Linux
# TODO: Testar no container Docker a integridade desse script.
# * OBS: Não faça uso desse script até eu ter certeza de que ele funciona corretamente (estou em um ambiente Windows).

add_folder_to_user_path() {
    local target_folder="$1"

    # Resolve o caminho para garantir que seja um caminho absoluto completo
    if ! absolute_caminho=$(readlink -f "$target_folder" 2>/dev/null); then
        echo -e "\e[31m[ERRO] A pasta especificada não existe: $target_folder\e[0m"
        return 1
    fi

    # Navega até a pasta (conforme solicitado)
    cd "$absolute_caminho" || return 1
    echo -e "\e[36m[INFO] Navegou para a pasta: $absolute_caminho\e[0m"

    # Define o arquivo de configuração do shell do usuário (.bashrc)
    local bash_profile="$HOME/.bashrc"
    
    # Se for macOS, o padrão geralmente é o .zshrc ou .bash_profile
    if [[ "$OSTYPE" == "darwin"* ]]; then
        bash_profile="$HOME/.bash_profile"
    fi

    # Verifica se a pasta já não está no PATH atual
    if [[ ":$PATH:" == *":$absolute_caminho:"* ]]; then
        echo -e "\e[33m[AVISO] Esta pasta já está registrada no seu PATH.\e[0m"
        return 0
    fi

    # Se não estiver lá, anexa o novo caminho ao arquivo de configuração para persistência
    try {
        echo "" >> "$bash_profile"
        echo "# Adicionado automaticamente pelo script de build" >> "$bash_profile"
        echo "export PATH=\"\$PATH:$absolute_caminho\"" >> "$bash_profile"
        
        echo -e "\e[32m[SUCESSO] A pasta foi adicionada ao PATH com sucesso!\e[0m"
        echo -e "\e[37mNota: Você precisará reiniciar o terminal ou rodar 'source $bash_profile' para que as alterações façam efeito.\e[0m"
        return 0
    } catch {
        echo -e "\e[31m[ERRO] Falha ao atualizar o PATH.\e[0m"
        return 1
    }
}

test_dotnet_installed() {
    # 'command -v' é o equivalente mais seguro e rápido ao 'Get-Command' ou 'which'
    if command -v dotnet >/dev/null 2>&1; then
        return 0 # No Bash, 0 significa sucesso/True
    else
        return 1 # 1 significa falha/False
    fi
}

# --- LÓGICA PRINCIPAL ---

# Obtém o diretório onde o script está localizado (equivalente ao $PSScriptRoot)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Define o caminho para a pasta build de forma absoluta
RELATIVE_PATH="$SCRIPT_DIR/build"

# Cria a pasta build caso ela não exista ainda para evitar erros de navegação inicial
mkdir -p "$RELATIVE_PATH"
cd "$RELATIVE_PATH" || exit 1

if ! test_dotnet_installed; then
    add_folder_to_user_path "$RELATIVE_PATH"
else
    cd "$SCRIPT_DIR/src" || { echo -e "\e[31m[ERRO] Pasta 'src' não encontrada.\e[0m"; exit 1; }
    
    dotnet build
    # Nota: Mantive o RID win-x64 do seu script original, altere para linux-x64 se for rodar nativo no Linux.
    dotnet publish -c Release -r win-x64 --self-contained true -o "$RELATIVE_PATH"
    
    cd "$RELATIVE_PATH" || exit 1
    add_folder_to_user_path "$RELATIVE_PATH"
fi

cd "$SCRIPT_DIR"

# Executa os comandos finais (certifique-se de que o 'ptml' está no escopo ou use ./build/ptml)
export PATH="$PATH:$RELATIVE_PATH" # Atualiza o PATH apenas para a sessão atual rodar os comandos abaixo
ptml --help --version
ptml run "index.ptml" --window