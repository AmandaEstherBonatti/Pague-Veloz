# PagueVeloz - Sistema de Processamento de Transações Financeiras

Sistema de processamento de transações financeiras desenvolvido com arquitetura DDD (Domain-Driven Design) em .NET 9.

## 🏗️ Arquitetura

O projeto segue os princípios de Clean Architecture e DDD, organizado em camadas:

- **Domain**: Entidades, Value Objects, Domain Services e Eventos de Domínio
- **Application**: Casos de uso, DTOs e Interfaces
- **Infrastructure**: Persistência (Entity Framework Core), Repositórios, Eventos e Serviços externos
- **Presentation**: Controllers e Middlewares

## 🚀 Funcionalidades

### Gestão de Clientes
- Cadastro de clientes
- Autenticação com JWT
- Suporte a autenticação multifator

### Gestão de Contas
- Criação de múltiplas contas por cliente
- Consulta de saldo disponível, reservado e total
- Limite de crédito configurável
- Bloqueio/desbloqueio de contas

### Processamento de Transações
- **Crédito**: Adiciona valor ao saldo disponível
- **Débito**: Subtrai valor do saldo (com suporte a limite de crédito)
- **Reserva**: Reserva valor do saldo disponível
- **Captura**: Confirma uma reserva
- **Estorno**: Reverte uma transação
- **Transferência**: Transfere entre contas

### Recursos Adicionais
- ✅ Idempotência via `ReferenceId`
- ✅ Validação de saldo e limite de crédito
- ✅ Eventos de domínio assíncronos
- ✅ Transações com rollback automático
- ✅ Consulta de extrato com filtros por período
- ✅ Logs estruturados (Serilog)
- ✅ Health Checks
- ✅ Retry com backoff exponencial
- ✅ Circuit Breaker
- ✅ Autenticação JWT

## 📋 Pré-requisitos

- .NET 9 SDK
- SQL Server (LocalDB ou SQL Server Express)
- Visual Studio 2022 ou VS Code

## 🔧 Configuração

### 1. Instalar ferramentas do Entity Framework

```bash
dotnet tool install --global dotnet-ef
```

### 2. Configurar Connection String

Edite `appsettings.json` e configure a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PagueVelozDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Criar e Aplicar Migrations

```bash
cd PagueVeloz
dotnet ef migrations add InitialCreate --project PagueVeloz.Infrastructure --startup-project PagueVeloz
dotnet ef database update --project PagueVeloz.Infrastructure --startup-project PagueVeloz
```

### 4. Executar a Aplicação

```bash
dotnet run --project PagueVeloz
```

A aplicação estará disponível em:
- HTTP: `http://localhost:5232`
- HTTPS: `https://localhost:7039`

## 📚 Endpoints da API

### Autenticação
- `POST /api/clientes` - Criar cliente
- `POST /api/clientes/login` - Login e obter token JWT

### Contas
- `POST /api/contas` - Criar conta
- `GET /api/contas/{id}` - Obter conta
- `GET /api/contas/cliente/{clienteId}` - Listar contas do cliente
- `GET /api/contas/{id}/saldo` - Consultar saldo

### Transações
- `POST /api/transacoes` - Processar transação
- `POST /api/transacoes/{id}/estornar` - Estornar transação
- `GET /api/transacoes/conta/{contaId}/extrato` - Obter extrato

### Health Checks
- `GET /health` - Health check geral
- `GET /health/ready` - Health check de prontidão
- `GET /health/live` - Health check de liveness

## 🔐 Autenticação JWT

Configure as opções JWT em `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForProduction!",
    "Issuer": "PagueVeloz",
    "Audience": "PagueVelozUsers",
    "ExpirationMinutes": "60"
  }
}
```

**Importante**: Em produção, use uma chave secreta forte e segura armazenada em variáveis de ambiente ou Azure Key Vault.

## 📝 Logs

Os logs são estruturados usando Serilog e são salvos em:
- Console (desenvolvimento)
- Arquivo: `logs/pagueveloz-YYYYMMDD.log`

## 🧪 Testes

Para executar os testes (quando implementados):

```bash
dotnet test
```

## 🏛️ Estrutura de Projetos

```
PagueVeloz/
├── PagueVeloz.Domain/          # Camada de Domínio
│   ├── Entities/              # Entidades
│   ├── ValueObjects/          # Value Objects
│   ├── Services/              # Domain Services
│   ├── Events/                # Domain Events
│   └── Enums/                 # Enumeradores
├── PagueVeloz.Application/    # Camada de Aplicação
│   ├── Services/              # Application Services
│   ├── DTOs/                  # Data Transfer Objects
│   └── Interfaces/           # Contratos
├── PagueVeloz.Infrastructure/ # Camada de Infraestrutura
│   ├── Persistence/           # DbContext e Configurações
│   ├── Repositories/          # Implementações de Repositórios
│   ├── Services/              # Serviços de Infraestrutura
│   ├── Events/                # Event Bus
│   ├── Middleware/            # Middlewares
│   └── Extensions/             # Extensões
└── PagueVeloz/                # Camada de Apresentação
    ├── Controllers/            # API Controllers
    └── Program.cs              # Configuração da aplicação
```

## 📦 Tecnologias Utilizadas

- .NET 9
- Entity Framework Core 9.0
- SQL Server
- JWT Bearer Authentication
- Serilog
- Polly (Retry e Circuit Breaker)
- ASP.NET Core Health Checks

## ✅ Funcionalidades Adicionais Implementadas

### Testes
- **Testes Unitários**: Projetos de teste criados com xUnit, Moq e FluentAssertions
- **Testes de Integração**: Estrutura preparada para testes end-to-end
- Exemplos de testes para Value Objects e Services

### Rate Limiting
- Rate limiting nativo do .NET 9
- Políticas configuráveis por endpoint:
  - **Global**: 100 requisições/minuto
  - **Autenticação**: 5 requisições/minuto por IP
  - **Transações**: 50 requisições/minuto por usuário
- Respostas HTTP 429 com informações de retry

### Cache Distribuído
- Interface `ICacheService` para abstração de cache
- Implementação com `MemoryCache` (pode ser trocada por Redis)
- Suporte a expiração e remoção de chaves

### Métricas
- Métricas usando `System.Diagnostics.Metrics`
- Contadores para:
  - Transações processadas/falhadas
  - Clientes e contas criadas
  - Tempo de processamento de transações
- Pronto para integração com Prometheus/Grafana

## 🔄 Próximos Passos Opcionais

- [ ] Expandir cobertura de testes
- [ ] Configurar CI/CD
- [ ] Integrar métricas com Prometheus/Grafana
- [ ] Implementar cache distribuído com Redis
- [ ] Implementar event sourcing (opcional)

## 📄 Licença

Este projeto é privado e proprietário.

