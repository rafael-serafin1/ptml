#!/usr/bin/env pwsh

function Add-FolderToUserPath {
    param (
        [Parameter(Mandatory = $true)]
        [string]$TargetFolder
    )

    # Resolve o caminho para garantir que caminhos relativos (como ".") virem caminhos absolutos
    $AbsoluteCaminho = Resolve-Path $TargetFolder -ErrorAction SilentlyContinue

    if (-not $AbsoluteCaminho) {
        Write-Host "[ERRO] A pasta especificada não existe: $TargetFolder" -ForegroundColor Red
        return $false
    }

    $PathToAdd = $AbsoluteCaminho.Path

    # Navega até a pasta (conforme solicitado)
    Set-Location -Path $PathToAdd
    Write-Host "[INFO] Navegou para a pasta: $PathToAdd" -ForegroundColor Cyan

    # Busca o PATH atual do Usuário diretamente do Registro do Windows
    $CurrentPath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::User)

    # Verifica se a pasta já não está no PATH (evita duplicatas)
    # Divide por ';' e limpa espaços em branco para uma comparação precisa
    $PathList = $CurrentPath -split ';' | ForEach-Object { $_.Trim() }

    if ($PathList -contains $PathToAdd) {
        Write-Host "[AVISO] Esta pasta ja esta registrada no seu PATH do Windows." -ForegroundColor Yellow
        return $true
    }

    # Se não estiver lá, anexa o novo caminho ao PATH existente
    $NewPath = "$CurrentPath;$PathToAdd"
    
    # Remove eventuais pontos e vírgulas duplicados que possam quebrar o PATH
    $NewPath = $NewPath -replace ';+', ';'

    try {
        # Salva o novo PATH no ambiente do Usuário
        [Environment]::SetEnvironmentVariable("Path", $NewPath, [EnvironmentVariableTarget]::User)
        
        Write-Host "[SUCESSO] A pasta foi adicionada ao PATH do Windows com sucesso!" -ForegroundColor Green
        Write-Host "Nota: Voce precisará reiniciar o terminal/VS Code para que as alterações façam efeito." -ForegroundColor White
        return $true
    }
    catch {
        Write-Host "[ERRO] Falha ao atualizar o PATH: $_" -ForegroundColor Red
        return $false
    }
}

function Test-DotNetInstalled {
    $command = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($command) {
        return $true
    } else {
        return $false
    }
}


<# Lógica principal #>
Set-Location ./build
if ($PSScriptRoot) {
    $RelativePath = Join-Path -Path $PSScriptRoot -ChildPath "build"
} else {
    $RelativePath = Join-Path -Path (Get-Location) -ChildPath "build"
}

if (-not (Test-DotNetInstalled)) {
    Add-FolderToUserPath -TargetFolder $RelativePath
} else {
    Set-Location ../src
    dotnet build
    dotnet publish -c Release -r win-x64 --self-contained true -o ./build
    Set-Location ../build
    Add-FolderToUserPath -TargetFolder $RelativePath
}

<# Verificação correta final #>
Set-Location ../
ptml --help --version
ptml run "index.ptml" --window