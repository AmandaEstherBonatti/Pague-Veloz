# 💰 PagueVeloz - Sistema de Processamento de Transações Financeiras

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18.2-61DAFB?logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.2-3178C6?logo=typescript)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)

**Sistema completo de gestão financeira com arquitetura moderna, escalável e segura**

[📖 Documentação](#-documentação) • [🚀 Início Rápido](#-início-rápido) • [🏗️ Arquitetura](#️-arquitetura) • [🔧 Configuração](#-configuração)

</div>

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Início Rápido](#-início-rápido)
- [Ambientes](#-ambientes)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Documentação](#-documentação)
- [Contribuindo](#-contribuindo)

---

## 🎯 Sobre o Projeto

O **PagueVeloz** é um sistema completo de processamento de transações financeiras desenvolvido com arquitetura moderna, seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**. 

O sistema permite gerenciar clientes, contas bancárias e processar diversos tipos de transações financeiras de forma segura, escalável e com alta disponibilidade.

### 🏗️ Arquitetura

O projeto é organizado em camadas bem definidas:

- **Domain**: Entidades, Value Objects, Domain Services e Eventos de Domínio
- **Application**: Casos de uso, DTOs e Interfaces
- **Infrastructure**: Persistência (Entity Framework Core), Repositórios, Eventos e Serviços externos
- **Presentation**: Controllers e Middlewares (API REST)

---

## ✨ Funcionalidades

### 👥 Gestão de Clientes
- ✅ Cadastro e autenticação de clientes
- ✅ Autenticação JWT com tokens seguros
- ✅ Suporte a autenticação multifator

### 💳 Gestão de Contas
- ✅ Criação de múltiplas contas por cliente
- ✅ Consulta de saldo disponível, reservado e total
- ✅ Limite de crédito configurável
- ✅ Bloqueio/desbloqueio de contas

### 💸 Processamento de Transações
- ✅ **Crédito**: Adiciona valor ao saldo disponível
- ✅ **Débito**: Subtrai valor do saldo (com suporte a limite de crédito)
- ✅ **Reserva**: Reserva valor do saldo disponível
- ✅ **Captura**: Confirma uma reserva
- ✅ **Estorno**: Reverte uma transação
- ✅ **Transferência**: Transfere entre contas

### 🚀 Recursos Adicionais
- ✅ Idempotência via `ReferenceId`
- ✅ Validação de saldo e limite de crédito
- ✅ Eventos de domínio assíncronos
- ✅ Transações com rollback automático
- ✅ Consulta de extrato com filtros por período
- ✅ Logs estruturados (Serilog)
- ✅ Health Checks
- ✅ Retry com backoff exponencial
- ✅ Circuit Breaker
- ✅ Rate Limiting
- ✅ Escalabilidade horizontal com Docker

---

## 🛠️ Tecnologias

### Backend
- **.NET 9** - Framework principal
- **ASP.NET Core** - API REST
- **Entity Framework Core 9.0** - ORM
- **SQL Server 2022** - Banco de dados
- **JWT Bearer Authentication** - Autenticação
- **Serilog** - Logging estruturado
- **Polly** - Retry e Circuit Breaker
- **ASP.NET Core Health Checks** - Monitoramento

### Frontend
- **React 18** - Biblioteca UI
- **TypeScript** - Tipagem estática
- **Vite** - Build tool e dev server
- **React Router** - Roteamento
- **Axios** - Cliente HTTP
- **Tailwind CSS** - Estilização
- **Lucide React** - Ícones
- **date-fns** - Formatação de datas

### Infraestrutura
- **Docker** - Containerização
- **Docker Compose** - Orquestração
- **Nginx** - Load balancer e proxy reverso
- **SQL Server (Container)** - Banco de dados

---

## 📦 Pré-requisitos

### Para Desenvolvimento Local (sem Docker)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express ou Developer)
- [Node.js 18+](https://nodejs.org/) e npm
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Para Docker
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (Windows/Mac) ou [Docker Engine](https://docs.docker.com/engine/install/) (Linux)
- [Docker Compose](https://docs.docker.com/compose/install/) (geralmente incluído no Docker Desktop)

---

## 🚀 Início Rápido

### Opção 1: Docker (Recomendado) 🐳

A forma mais rápida de subir toda a aplicação:

```bash
# 1. Clone o repositório
git clone <url-do-repositorio>
cd Pague-Veloz

# 2. Crie o arquivo .env (veja a seção de Configuração)
cp .env.example .env
# Edite o .env com suas configurações

# 3. Suba todos os serviços
docker-compose up -d

# 4. Aguarde os serviços iniciarem (pode levar alguns minutos na primeira vez)
docker-compose logs -f

# 5. Acesse a aplicação
# Frontend: http://localhost
# API: http://localhost/api
# Swagger: http://localhost/api/swagger
```

### Opção 2: Desenvolvimento Local 💻

#### Backend

```bash
# 1. Navegue até a pasta do backend
cd PagueVeloz

# 2. Restaure as dependências
dotnet restore

# 3. Configure a connection string no appsettings.json
# Edite: ConnectionStrings__DefaultConnection

# 4. Execute as migrations (se necessário)
dotnet ef database update

# 5. Execute a API
dotnet run

# A API estará disponível em: http://localhost:5232
# Swagger: http://localhost:5232/swagger
```

#### Frontend

```bash
# 1. Navegue até a pasta do frontend
cd PagueVeloz.Frontend

# 2. Instale as dependências
npm install

# 3. Crie o arquivo .env (opcional)
# VITE_API_BASE_URL=http://localhost:5232/api

# 4. Execute o servidor de desenvolvimento
npm run dev

# O frontend estará disponível em: http://localhost:3000
```

---

## 🌍 Ambientes

### 🔧 Desenvolvimento

Ambiente configurado para desenvolvimento local com hot-reload e debug facilitado.

```bash
# Usa docker-compose.yml + docker-compose.override.yml automaticamente
docker-compose up -d

# Ou explicitamente:
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

**Características:**
- 1 instância da API
- Porta 8080 exposta para debug
- Logs detalhados
- Volumes montados para desenvolvimento

**Acessos:**
- Frontend: `http://localhost`
- API: `http://localhost:8080` (direto) ou `http://localhost/api` (via Nginx)
- Swagger: `http://localhost/api/swagger`
- Banco: `localhost:1433`

### 🚀 Produção

Ambiente otimizado para produção com escalabilidade horizontal.

```bash
# Subir com configuração de produção (3 instâncias da API)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Ou escalar manualmente
docker-compose up -d --scale api=5
```

**Características:**
- 3+ instâncias da API (configurável)
- Load balancing com Nginx
- Health checks ativos
- Recursos otimizados
- Logs estruturados

**Acessos:**
- Frontend: `http://localhost` (ou seu domínio)
- API: `http://localhost/api`
- Swagger: `http://localhost/api/swagger` (desabilitar em produção real)

### 📈 Alta Escala

Para ambientes com alta carga de requisições:

```bash
# Escalar para 10 instâncias
docker-compose -f docker-compose.yml -f docker-compose.scale.yml up -d --scale api=10
```

**Recomendações por carga:**
- **< 100 req/s**: 1-2 instâncias
- **100-500 req/s**: 2-3 instâncias
- **500-1000 req/s**: 3-5 instâncias
- **1000-5000 req/s**: 5-10 instâncias
- **> 5000 req/s**: 10+ instâncias + Kubernetes

---

## ⚙️ Configuração

### Variáveis de Ambiente (.env)

Crie um arquivo `.env` na raiz do projeto:

```env
# Database
DB_PASSWORD=SuaSenhaForteAqui123!
DB_PORT=1433

# JWT
JWT_SECRET_KEY=SuaChaveSecretaComPeloMenos32CaracteresParaProdução!
JWT_ISSUER=PagueVeloz
JWT_AUDIENCE=PagueVelozUsers
JWT_EXPIRATION_MINUTES=60

# Nginx
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443

# API
ASPNETCORE_ENVIRONMENT=Production
```

### Connection String

Para desenvolvimento local (sem Docker), edite `PagueVeloz/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PagueVelozDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Frontend

Crie `PagueVeloz.Frontend/.env`:

```env
VITE_API_BASE_URL=http://localhost:5232/api
```

---

## 📁 Estrutura do Projeto

```
Pague-Veloz/
├── 📁 PagueVeloz/                    # Backend (.NET)
│   ├── 📁 PagueVeloz.Domain/         # Camada de Domínio
│   │   ├── Entities/                 # Entidades
│   │   ├── ValueObjects/             # Value Objects
│   │   ├── Services/                 # Domain Services
│   │   ├── Events/                   # Domain Events
│   │   └── Enums/                    # Enumeradores
│   ├── 📁 PagueVeloz.Application/    # Camada de Aplicação
│   │   ├── Services/                 # Application Services
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   └── Interfaces/               # Contratos
│   ├── 📁 PagueVeloz.Infrastructure/ # Camada de Infraestrutura
│   │   ├── Persistence/              # DbContext e Configurações
│   │   ├── Repositories/              # Implementações de Repositórios
│   │   ├── Services/                  # Serviços de Infraestrutura
│   │   ├── Events/                    # Event Bus
│   │   └── Middleware/                # Middlewares
│   ├── 📁 PagueVeloz/                # Camada de Apresentação
│   │   ├── Controllers/               # API Controllers
│   │   └── Program.cs                 # Configuração da aplicação
│   └── 📁 PagueVeloz.*.Tests/        # Projetos de Testes
│
├── 📁 PagueVeloz.Frontend/           # Frontend (React + TypeScript)
│   ├── src/
│   │   ├── components/               # Componentes React
│   │   ├── pages/                    # Páginas
│   │   ├── services/                 # Serviços API
│   │   ├── contexts/                 # Contexts React
│   │   └── types/                    # Tipos TypeScript
│   └── package.json
│
├── 📁 nginx/                          # Configuração Nginx
│   ├── nginx.conf                     # Configuração global
│   └── conf.d/
│       └── pagueveloz.conf            # Virtual host
│
├── 📁 scripts/                        # Scripts auxiliares
│   ├── build.sh / build.ps1          # Build
│   ├── docker-compose-scale.sh        # Escalar serviços
│   └── docker-health.sh               # Health check
│
├── 📁 logs/                           # Logs da aplicação
│
├── docker-compose.yml                 # Configuração base
├── docker-compose.override.yml        # Override para desenvolvimento
├── docker-compose.prod.yml            # Override para produção
├── docker-compose.scale.yml           # Override para alta escala
│
└── README.md                          # Este arquivo
```

---

## 📚 Documentação

### Documentação Adicional

- [📖 Guia Docker e Escalabilidade](./DOCKER.md) - Detalhes sobre Docker e escalabilidade
- [📋 Guia de Deploy](./docs/Guia_PagueVeloz.md) - Guia completo de deploy
- [✅ Validação de Requisitos](./VALIDACAO_REQUISITOS.md) - Checklist de requisitos
- [🧪 Testes](./PagueVeloz/TESTES.md) - Documentação de testes

### Comandos Úteis

#### Docker

```bash
# Ver logs
docker-compose logs -f api
docker-compose logs -f nginx

# Ver status dos containers
docker-compose ps

# Parar serviços
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Rebuild
docker-compose up -d --build

# Escalar API
docker-compose up -d --scale api=3

# Health check
curl http://localhost/api/health
```

#### Backend

```bash
# Restaurar dependências
dotnet restore

# Build
dotnet build

# Executar testes
dotnet test

# Executar migrations
dotnet ef database update

# Criar migration
dotnet ef migrations add NomeDaMigration
```

#### Frontend

```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview da build
npm run preview

# Lint
npm run lint
```

---

## 🔒 Segurança

### ⚠️ Importante

1. **Nunca commite arquivos `.env`** - Use `.env.example` como template
2. **Altere todas as senhas padrão** antes de usar em produção
3. **Use secrets** em produção (Docker Swarm/Kubernetes)
4. **Configure HTTPS** no Nginx para produção
5. **Restrinja portas** expostas ao necessário
6. **Use variáveis de ambiente** para todos os secrets

### Checklist de Segurança

- [ ] Senha do banco alterada
- [ ] JWT Secret Key alterada (mínimo 32 caracteres)
- [ ] Arquivo `.env` no `.gitignore`
- [ ] HTTPS configurado (produção)
- [ ] Rate limiting ativo
- [ ] CORS configurado corretamente
- [ ] Logs não expõem informações sensíveis

---

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Testes com cobertura
dotnet test /p:CollectCoverage=true

# Testes específicos
dotnet test --filter "FullyQualifiedName~ClienteServiceTests"
```

---

## 📊 Monitoramento

### Health Checks

```bash
# Verificar saúde da API
curl http://localhost/api/health

# Script automático
./scripts/docker-health.sh
```

### Logs

```bash
# Logs em tempo real
docker-compose logs -f api

# Últimas 100 linhas
docker-compose logs --tail=100 api

# Logs do arquivo
tail -f logs/pagueveloz-*.log
```

### Métricas

```bash
# Uso de recursos
docker stats

# Status dos containers
docker-compose ps
```

---

## 🐛 Troubleshooting

### API não inicia

```bash
# Verificar logs
docker-compose logs api

# Verificar conexão com banco
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P '${DB_PASSWORD}' -Q "SELECT 1"
```

### Frontend não conecta à API

1. Verifique se a API está rodando
2. Verifique a URL no `.env` do frontend
3. Verifique CORS no backend
4. Verifique o proxy no `vite.config.ts`

### Banco de dados não conecta

1. Verifique se o container está rodando: `docker-compose ps sqlserver`
2. Verifique a connection string
3. Verifique as variáveis de ambiente no `.env`
4. Verifique os logs: `docker-compose logs sqlserver`

### Nginx não balanceia

```bash
# Verificar configuração
docker-compose exec nginx cat /etc/nginx/conf.d/pagueveloz.conf

# Reiniciar Nginx
docker-compose restart nginx
```

---

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## 👥 Autores

- **Equipe PagueVeloz** - *Desenvolvimento inicial*

---

## 🙏 Agradecimentos

- Comunidade .NET
- Comunidade React
- Todos os contribuidores de código aberto

---

<div align="center">

**Desenvolvido com ❤️ usando .NET 9, React e Docker**

⭐ Se este projeto foi útil, considere dar uma estrela!

</div>
