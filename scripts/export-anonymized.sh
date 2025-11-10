#!/bin/bash

# Script para exportar solução anonimizada
# Gera um arquivo ZIP sem informações do histórico Git

echo "📦 Gerando arquivo ZIP anonimizado..."

# Verificar se está na raiz do repositório
if [ ! -d ".git" ]; then
    echo "❌ Erro: Execute este script na raiz do repositório Git"
    exit 1
fi

# Nome do arquivo de saída
OUTPUT_FILE="pagueveloz-challenge.zip"

# Remover arquivo existente se houver
if [ -f "$OUTPUT_FILE" ]; then
    rm -f "$OUTPUT_FILE"
    echo "🗑️  Arquivo existente removido"
fi

# Gerar arquivo ZIP usando git archive
echo "📤 Exportando arquivos do repositório..."
git archive --format=zip --output="$OUTPUT_FILE" HEAD

if [ $? -eq 0 ]; then
    FILE_SIZE=$(du -h "$OUTPUT_FILE" | cut -f1)
    echo "✅ Arquivo gerado com sucesso: $OUTPUT_FILE ($FILE_SIZE)"
    echo ""
    echo "📋 Verificações recomendadas antes de submeter:"
    echo "   - Verifique se não há informações pessoais no README.md"
    echo "   - Confirme que arquivos .env estão no .gitignore"
    echo "   - Remova comentários com nomes ou emails do código"
    echo "   - Verifique se não há caminhos de usuário hardcoded"
else
    echo "❌ Erro ao gerar arquivo ZIP"
    exit 1
fi

