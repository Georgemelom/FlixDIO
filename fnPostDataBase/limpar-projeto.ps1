# ============================================
# Script de limpeza total do projeto .NET
# ============================================

param (
    [string]$Path = "C:\Temp\FlixDIO\fnPostDataBase"
)

Write-Host "Limpando projeto em: $Path..."

# Verifica se o caminho existe
if (-Not (Test-Path $Path)) {
    Write-Host "Caminho não encontrado: $Path"
    exit
}

# Fecha possíveis processos que bloqueiam os arquivos (silencioso)
Get-Process | Where-Object { $_.Path -like "$Path*" } | ForEach-Object {
    try {
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
        Write-Host "Processo encerrado: $($_.ProcessName)"
    } catch { }
}

# Remove bin e obj (forçando exclusão)
Write-Host "Removendo pastas bin e obj..."
Get-ChildItem -Path $Path -Include bin, obj -Recurse -Force -ErrorAction SilentlyContinue |
    ForEach-Object {
        try {
            Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
            Write-Host "Removido: $($_.FullName)"
        } catch {
            Write-Host "Falha ao remover: $($_.FullName)"
        }
    }

# Limpa cache do NuGet
Write-Host "Limpando cache do NuGet..."
dotnet nuget locals all --clear | Out-Null

# Faz limpeza do projeto .NET
Write-Host "Executando dotnet clean..."
dotnet clean $Path | Out-Null

Write-Host "Limpeza concluída com sucesso!"
