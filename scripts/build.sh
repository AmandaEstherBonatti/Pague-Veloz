#!/bin/bash

# Script para build e deploy da aplicação Docker

set -e

echo "🐳 Building PagueVeloz Docker images..."

# Build da API
echo "📦 Building API image..."
docker build -t pagueveloz-api:latest ./PagueVeloz

echo "✅ Build completed successfully!"

# Para executar:
# docker-compose up -d

# Para escalar:
# docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=3

