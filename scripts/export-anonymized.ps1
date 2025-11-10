# Script para exportar solução anonimizada
# Gera um arquivo ZIP sem informações do histórico Git

Write-Host "📦 Gerando arquivo ZIP anonimizado..." -ForegroundColor Cyan

# Verificar se está na raiz do repositório
if (-not (Test-Path ".git")) {
    Write-Host "❌ Erro: Execute este script na raiz do repositório Git" -ForegroundColor Red
    exit 1
}

# Nome do arquivo de saída
$outputFile = "pagueveloz-challenge.zip"

# Remover arquivo existente se houver
if (Test-Path $outputFile) {
    Remove-Item $outputFile -Force
    Write-Host "🗑️  Arquivo existente removido" -ForegroundColor Yellow
}

# Gerar arquivo ZIP usando git archive
Write-Host "📤 Exportando arquivos do repositório..." -ForegroundColor Cyan
git archive --format=zip --output=$outputFile HEAD

if ($LASTEXITCODE -eq 0) {
    $fileSize = (Get-Item $outputFile).Length / 1MB
    Write-Host "✅ Arquivo gerado com sucesso: $outputFile ($([math]::Round($fileSize, 2)) MB)" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 Verificações recomendadas antes de submeter:" -ForegroundColor Yellow
    Write-Host "   - Verifique se não há informações pessoais no README.md" -ForegroundColor White
    Write-Host "   - Confirme que arquivos .env estão no .gitignore" -ForegroundColor White
    Write-Host "   - Remova comentários com nomes ou emails do código" -ForegroundColor White
    Write-Host "   - Verifique se não há caminhos de usuário hardcoded" -ForegroundColor White
} else {
    Write-Host "❌ Erro ao gerar arquivo ZIP" -ForegroundColor Red
    exit 1
}

