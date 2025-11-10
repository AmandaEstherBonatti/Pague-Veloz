# 🐳 Docker - Guia de Escalabilidade

Este guia explica como escalar a aplicação PagueVeloz usando Docker.

## 📋 Índice

- [Início Rápido](#início-rápido)
- [Escalabilidade](#escalabilidade)
- [Arquitetura](#arquitetura)
- [Configuração](#configuração)
- [Monitoramento](#monitoramento)
- [Troubleshooting](#troubleshooting)

## 🚀 Início Rápido

### 1. Desenvolvimento Local

```bash
# Criar arquivo .env
cp .env.example .env

# Iniciar serviços (1 instância)
docker-compose up -d

# Ver logs
docker-compose logs -f api
```

### 2. Produção com Escalabilidade

```bash
# Iniciar com 3 instâncias
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Ou escalar dinamicamente
docker-compose up -d --scale api=5
```

## 📈 Escalabilidade

### Escalar Horizontalmente

A API é **stateless** (sem estado), permitindo escalar horizontalmente adicionando mais instâncias.

#### Escalar para 3 instâncias

```bash
# Método 1: Script (Recomendado)
./scripts/docker-compose-scale.sh 3

# Método 2: Docker Compose
docker-compose up -d --scale api=3

# Método 3: Arquivo de produção
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

#### Escalar para 5+ instâncias

```bash
# Alta escala
docker-compose up -d --scale api=5

# Muito alta escala
docker-compose -f docker-compose.yml -f docker-compose.scale.yml up -d --scale api=10
```

### Como Funciona?

1. **Nginx Load Balancer**: Distribui requisições entre instâncias usando `least_conn`
2. **Service Discovery**: Docker Compose resolve automaticamente `api:8080` para todas as instâncias
3. **Health Checks**: Instâncias não saudáveis são removidas do balanceamento
4. **Stateless API**: Cada requisição é independente

### Verificar Escalagem

```bash
# Ver todas as instâncias
docker-compose ps api

# Ver logs de todas as instâncias
docker-compose logs -f api

# Verificar health
./scripts/docker-health.sh
```

## 🏗️ Arquitetura

```
                    ┌─────────────┐
                    │   Cliente   │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │   Nginx     │  ← Load Balancer (Porta 80)
                    │  (Port 80)  │
                    └──────┬──────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
   ┌────▼────┐      ┌──────▼────┐      ┌─────▼─────┐
   │ API #1  │      │  API #2   │      │  API #3   │
   │ :8080   │      │  :8080    │      │  :8080    │
   └────┬────┘      └──────┬────┘      └─────┬─────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
                    ┌──────▼──────┐
                    │ SQL Server  │  ← Database (Porta 1433)
                    │             │
                    └─────────────┘
```

## ⚙️ Configuração

### Variáveis de Ambiente

Crie `.env` a partir de `.env.example`:

```bash
cp .env.example .env
```

Principais configurações:

```env
# Database
DB_PASSWORD=sua_senha_forte_aqui
DB_PORT=1433

# JWT
JWT_SECRET_KEY=sua_chave_secreta_32_chars_minimo

# API Scaling
API_REPLICAS=3
```

### Recursos por Instância

**Desenvolvimento** (docker-compose.yml):
- CPU: 0.5-1 core
- Memória: 256-512MB

**Produção** (docker-compose.prod.yml):
- CPU: 1-2 cores
- Memória: 512MB-1GB

**Alta Escala** (docker-compose.scale.yml):
- CPU: 0.75-1.5 cores
- Memória: 384-768MB

## 📊 Monitoramento

### Health Checks

```bash
# Script automático
./scripts/docker-health.sh

# Manual
curl http://localhost/api/health
```

### Logs

```bash
# Todas as instâncias
docker-compose logs -f api

# Instância específica
docker-compose logs -f pagueveloz-api-1

# Últimas 100 linhas
docker-compose logs --tail=100 api
```

### Recursos

```bash
# Uso de recursos em tempo real
docker stats

# Status dos containers
docker-compose ps
```

## 🔍 Troubleshooting

### API não escala

```bash
# Remover container_name do docker-compose.yml
# Container_name impede escalagem

# Verificar se está usando service name
docker-compose ps api
```

### Load balancer não distribui

```bash
# Verificar configuração do Nginx
docker-compose exec nginx cat /etc/nginx/conf.d/pagueveloz.conf

# Reiniciar Nginx
docker-compose restart nginx
```

### Muitas conexões no banco

```bash
# Ajustar connection pool no connection string
# Max Pool Size=100 (padrão)
# Min Pool Size=5 (padrão)

# Verificar conexões ativas
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P '${DB_PASSWORD}' \
  -Q "SELECT COUNT(*) FROM sys.dm_exec_connections"
```

### Performance

```bash
# Aumentar workers do Nginx
# Editar nginx/nginx.conf: worker_processes auto;

# Aumentar worker_connections
# Editar nginx/nginx.conf: worker_connections 2048;

# Reiniciar
docker-compose restart nginx
```

## 📚 Comandos Úteis

### Escalar

```bash
# Escalar para N instâncias
docker-compose up -d --scale api=N

# Escalar com produção
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=5
```

### Atualizar

```bash
# Rebuild e restart
docker-compose up -d --build --scale api=3

# Rolling update (sem downtime)
docker-compose up -d --no-deps --build api
```

### Limpar

```bash
# Parar e remover
docker-compose down

# Remover volumes também
docker-compose down -v

# Limpar imagens
docker image prune -a
```

## 🎯 Recomendações

### Por Carga de Trabalho

- **< 100 req/s**: 1-2 instâncias
- **100-500 req/s**: 2-3 instâncias
- **500-1000 req/s**: 3-5 instâncias
- **1000-5000 req/s**: 5-10 instâncias
- **> 5000 req/s**: 10+ instâncias + Kubernetes

### Otimizações

1. **Connection Pooling**: Ajustado automaticamente
2. **Caching**: In-memory cache habilitado
3. **Rate Limiting**: Nginx + ASP.NET Core
4. **Gzip**: Compressão habilitada
5. **Keep-Alive**: Conexões reutilizadas

## 🔒 Segurança

1. **Altere senhas padrão** no `.env`
2. **Use secrets** em produção (Docker Swarm/Kubernetes)
3. **Configure HTTPS** no Nginx
4. **Restrinja portas** expostas
5. **Use variáveis de ambiente** para secrets

---

**✅ Aplicação pronta para escalar horizontalmente!**

