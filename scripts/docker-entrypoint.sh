#!/bin/sh

# Script de inicialização do container
# Aguarda o banco de dados estar pronto e executa migrations

echo "⏳ Waiting for SQL Server to be ready..."

# Aguarda até 60 segundos pelo SQL Server estar disponível
for i in $(seq 1 60); do
    if nc -z sqlserver 1433; then
        echo "✅ SQL Server is ready!"
        break
    fi
    echo "Waiting for SQL Server... ($i/60)"
    sleep 1
done

echo "🚀 Starting PagueVeloz API..."

# Executa a aplicação
exec dotnet PagueVeloz.dll

