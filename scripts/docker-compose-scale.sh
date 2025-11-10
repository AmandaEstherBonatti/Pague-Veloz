#!/bin/bash

# Script para escalar a aplicação PagueVeloz

set -e

REPLICAS=${1:-3}

echo "🚀 Escalando PagueVeloz para $REPLICAS instâncias..."

# Verificar se os serviços estão rodando
if ! docker-compose ps | grep -q "Up"; then
    echo "❌ Serviços não estão rodando. Iniciando..."
    docker-compose up -d
fi

# Escalar API
echo "📈 Escalando API para $REPLICAS instâncias..."
docker-compose up -d --scale api=$REPLICAS --no-recreate

# Aguardar health checks
echo "⏳ Aguardando health checks..."
sleep 10

# Verificar status
echo "📊 Status dos serviços:"
docker-compose ps

echo "✅ Escalagem concluída!"
echo "🌐 Acesse: http://localhost/api/health"

