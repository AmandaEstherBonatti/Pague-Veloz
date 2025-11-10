# 🐳 Docker Setup - PagueVeloz

Este documento descreve como executar e escalar a aplicação PagueVeloz usando Docker.

## 📋 Pré-requisitos

- Docker Desktop (Windows/Mac) ou Docker Engine (Linux)
- Docker Compose v2.0+
- 4GB+ RAM disponível

## 🚀 Início Rápido

### 1. Desenvolvimento Local

```bash
# Build e iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar serviços
docker-compose down
```

### 2. Produção com Escalabilidade

```bash
# Build da imagem
docker build -t pagueveloz-api:latest ./PagueVeloz

# Iniciar com 3 instâncias da API (load balancing automático)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=3

# Verificar status
docker-compose ps
```

## 🏗️ Arquitetura

```
┌─────────────┐
│   Nginx     │  ← Load Balancer / Reverse Proxy (Porta 80)
│  (Port 80)  │
└──────┬──────┘
       │
       ├───► API Instance 1 (Port 8080)
       ├───► API Instance 2 (Port 8080)
       └───► API Instance 3 (Port 8080)
              │
              ▼
       ┌─────────────┐
       │  SQL Server │  ← Database (Port 1433)
       │             │
       └─────────────┘
```

## 📦 Serviços

### 1. **API (pagueveloz-api)**
- **Porta**: 8080 (interno), 80 via Nginx (externo)
- **Escalável**: Sim (use `--scale api=N`)
- **Health Check**: `/health`
- **Variáveis de Ambiente**: Ver `.env.example`

### 2. **SQL Server (pagueveloz-db)**
- **Porta**: 1433
- **Usuário**: sa
- **Senha**: Definida em `docker-compose.yml`
- **Volume**: Persistência de dados em volume Docker

### 3. **Nginx (pagueveloz-nginx)**
- **Porta**: 80 (HTTP), 443 (HTTPS - configurar certificados)
- **Função**: Load balancer e reverse proxy
- **Rate Limiting**: Configurado
- **CORS**: Configurado

## 🔧 Configuração

### Variáveis de Ambiente

Crie um arquivo `.env` baseado em `.env.example`:

```bash
cp .env.example .env
```

Edite `.env` com suas configurações:

```env
# Database
DB_PASSWORD=sua_senha_forte_aqui
DB_PORT=1433

# JWT
JWT_SECRET_KEY=sua_chave_secreta_aqui
JWT_ISSUER=PagueVeloz
JWT_AUDIENCE=PagueVelozUsers
JWT_EXPIRATION_MINUTES=60

# Nginx
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443

# API Scaling
API_REPLICAS=3
```

### Escalando a Aplicação

#### Escala Horizontal (Múltiplas Instâncias)

**Método 1: Usando scripts (Recomendado)**

```bash
# Linux/Mac
./scripts/docker-compose-scale.sh 3

# Windows PowerShell
.\scripts\docker-compose-scale.ps1 -Replicas 3
```

**Método 2: Docker Compose diretamente**

```bash
# Escalar para 3 instâncias
docker-compose up -d --scale api=3

# Escalar para 5 instâncias
docker-compose up -d --scale api=5

# Escalar para 10 instâncias (alta carga)
docker-compose up -d --scale api=10

# Verificar instâncias rodando
docker-compose ps api

# Ver logs de todas as instâncias
docker-compose logs -f api
```

**Método 3: Usando arquivo de produção**

```bash
# Usar configuração de produção (3 instâncias)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Escalar para mais instâncias
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=5
```

**Método 4: Alta escala**

```bash
# Escalar para 10 instâncias (alta carga)
docker-compose -f docker-compose.yml -f docker-compose.scale.yml up -d --scale api=10
```

#### Escala Vertical (Recursos)

Edite `docker-compose.prod.yml` para ajustar limites de CPU e memória:

```yaml
resources:
  limits:
    cpus: '2'
    memory: 1G
  reservations:
    cpus: '1'
    memory: 512M
```

## 🚀 Escalabilidade

### Por que a arquitetura é escalável?

1. **Stateless API**: A API não mantém estado, permitindo múltiplas instâncias
2. **Load Balancing**: Nginx distribui requisições entre instâncias
3. **Service Discovery**: Docker Compose resolve automaticamente múltiplas instâncias
4. **Connection Pooling**: EF Core gerencia pool de conexões com o banco
5. **Health Checks**: Containers não saudáveis são removidos do load balancer

### Recomendações de Escala

- **Desenvolvimento**: 1 instância
- **Produção pequena**: 2-3 instâncias
- **Produção média**: 3-5 instâncias
- **Alta carga**: 5-10 instâncias
- **Muito alta carga**: 10+ instâncias (considere Kubernetes)

### Monitoramento de Performance

```bash
# Ver uso de recursos em tempo real
docker stats

# Ver logs de todas as instâncias
docker-compose logs -f api

# Verificar balanceamento
watch -n 1 'curl -s http://localhost/api/health | jq'
```

## 📊 Monitoramento

### Health Checks

```bash
# Usar script de health check
./scripts/docker-health.sh

# Ou manualmente:
# Health check da API
curl http://localhost/api/health

# Health check do banco
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P '${DB_PASSWORD}' -Q 'SELECT 1'

# Verificar todas as instâncias da API
docker-compose ps api
```

### Logs

```bash
# Todos os serviços
docker-compose logs -f

# Apenas API
docker-compose logs -f api

# Últimas 100 linhas
docker-compose logs --tail=100 api

# Logs de todas as instâncias da API
docker-compose logs -f api
```

### Status dos Containers

```bash
# Status geral
docker-compose ps

# Uso de recursos
docker stats

# Informações detalhadas
docker-compose top
```

## 🔄 Comandos Úteis

### Build e Deploy

```bash
# Build da imagem
docker build -t pagueveloz-api:latest ./PagueVeloz

# Build sem cache
docker build --no-cache -t pagueveloz-api:latest ./PagueVeloz

# Rebuild e restart
docker-compose up -d --build

# Restart apenas um serviço
docker-compose restart api
```

### Database Migrations

```bash
# Executar migrations (dentro do container)
docker-compose exec api dotnet ef database update

# Ou criar migration
docker-compose exec api dotnet ef migrations add NomeDaMigration
```

### Limpeza

```bash
# Parar e remover containers
docker-compose down

# Remover volumes também (⚠️ apaga dados!)
docker-compose down -v

# Limpar imagens não utilizadas
docker image prune -a

# Limpar tudo (⚠️ cuidado!)
docker system prune -a --volumes
```

## 🔒 Segurança

### Produção

1. **Altere senhas padrão** em `docker-compose.yml`
2. **Use secrets** do Docker Swarm ou Kubernetes para credenciais
3. **Configure HTTPS** no Nginx (certificados SSL)
4. **Restrinja portas** expostas
5. **Use variáveis de ambiente** para secrets

### Exemplo com Secrets (Docker Swarm)

```bash
# Criar secret
echo "minha_senha_secreta" | docker secret create db_password -

# Usar no docker-compose
# secrets:
#   db_password:
#     external: true
```

## 🌐 Acesso

Após iniciar os serviços:

- **API via Nginx**: http://localhost/api
- **Health Check**: http://localhost/health
- **Swagger** (dev): http://localhost/swagger
- **SQL Server**: localhost:1433

## 📈 Performance

### Otimizações Implementadas

1. **Multi-stage build** - Imagem final menor
2. **Non-root user** - Segurança
3. **Health checks** - Auto-recovery
4. **Load balancing** - Distribuição de carga (Nginx)
5. **Connection pooling** - EF Core
6. **Caching** - In-memory cache
7. **Rate limiting** - Nginx e ASP.NET Core

### Benchmarks

Para testar performance:

```bash
# Instalar Apache Bench (ab)
# Ubuntu/Debian: apt-get install apache2-utils
# Mac: brew install httpd

# Teste de carga
ab -n 1000 -c 10 http://localhost/api/health
```

## 🐛 Troubleshooting

### API não inicia

```bash
# Ver logs
docker-compose logs api

# Verificar se o banco está pronto
docker-compose ps sqlserver

# Restart
docker-compose restart api
```

### Erro de conexão com banco

```bash
# Verificar se o SQL Server está rodando
docker-compose ps sqlserver

# Testar conexão
docker-compose exec api ping sqlserver

# Ver logs do SQL Server
docker-compose logs sqlserver
```

### Porta já em uso

```bash
# Verificar o que está usando a porta
# Windows: netstat -ano | findstr :80
# Linux/Mac: lsof -i :80

# Alterar porta no docker-compose.yml
ports:
  - "8080:80"  # Em vez de "80:80"
```

## 📚 Recursos Adicionais

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Nginx Load Balancing](https://nginx.org/en/docs/http/load_balancing.html)
- [ASP.NET Core Docker](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)

## 🤝 Contribuindo

Para adicionar novos serviços ou configurar SSL:

1. Adicione o serviço em `docker-compose.yml`
2. Configure o Nginx em `nginx/conf.d/`
3. Atualize este README

---

**Desenvolvido com ❤️ para escalabilidade e performance**

