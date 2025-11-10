#!/bin/bash

# Script para verificar saúde dos serviços Docker

set -e

echo "🏥 Verificando saúde dos serviços PagueVeloz..."
echo ""

# Verificar containers
echo "📦 Containers:"
docker-compose ps
echo ""

# Health check da API
echo "🔍 Health Check da API:"
API_HEALTH=$(curl -s -o /dev/null -w "%{http_code}" http://localhost/api/health || echo "000")
if [ "$API_HEALTH" = "200" ]; then
    echo "✅ API está saudável (HTTP $API_HEALTH)"
else
    echo "❌ API não está respondendo (HTTP $API_HEALTH)"
fi
echo ""

# Verificar instâncias da API
echo "📊 Instâncias da API:"
API_COUNT=$(docker-compose ps api | grep -c "Up" || echo "0")
echo "   $API_COUNT instância(s) rodando"
echo ""

# Verificar SQL Server
echo "🗄️  SQL Server:"
DB_STATUS=$(docker-compose ps sqlserver | grep -o "Up" || echo "Down")
if [ "$DB_STATUS" = "Up" ]; then
    echo "✅ SQL Server está rodando"
else
    echo "❌ SQL Server não está rodando"
fi
echo ""

# Verificar Nginx
echo "🌐 Nginx:"
NGINX_STATUS=$(docker-compose ps nginx | grep -o "Up" || echo "Down")
if [ "$NGINX_STATUS" = "Up" ]; then
    echo "✅ Nginx está rodando"
else
    echo "❌ Nginx não está rodando"
fi
echo ""

# Uso de recursos
echo "💻 Uso de recursos:"
docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}"

