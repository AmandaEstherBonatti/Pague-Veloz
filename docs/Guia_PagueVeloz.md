# Guia de Deploy PagueVeloz

## Visão Geral do Projeto

A solução **PagueVeloz** é composta por:

- **Backend (`PagueVeloz/`)**: API ASP.NET Core dividida em camadas Domain, Application, Infrastructure e Presentation, seguindo princípios de Clean Architecture e DDD.
- **Frontend (`PagueVeloz.Frontend/`)**: SPA em React + TypeScript + Vite + Tailwind.
- **Infraestrutura Docker**: `docker-compose*.yml`, `Dockerfile`s e configuração de Nginx para orquestração, proxy reverso e balanceamento.
- **Banco de Dados**: SQL Server em container, com dados persistidos em volume Docker.
- **Scripts auxiliares**: `scripts/` para build, escala, health-check.

## Estrutura das Pastas

- `.vs`: arquivos gerados pelo Visual Studio.
- `nginx/`: configuração global (`nginx.conf`) e virtual host (`conf.d/pagueveloz.conf`) com rate limiting, CORS, cache e roteamento para API/frontend.
- `PagueVeloz/`: solução backend com subprojetos:
  - `PagueVeloz.Application/`: serviços de aplicação, DTOs, interfaces.
  - `PagueVeloz.Domain/`: entidades, value objects, eventos e serviços de domínio.
  - `PagueVeloz.Infrastructure/`: repositórios, persistência, cache, middleware, métricas.
  - `PagueVeloz.Application.Tests`, `Domain.Tests`, `Integration.Tests`: testes unitários/integrados.
  - `Dockerfile` e `Dockerfile.dev`: imagens de build e dev.
- `PagueVeloz.Frontend/`: páginas, componentes, contexts, services e configs (Vite, Tailwind, TS).
- `scripts/`: utilitários PowerShell/Bash para build, escala e health-check.
- `docker-compose.yml`, `docker-compose.override.yml`, `docker-compose.prod.yml`, `docker-compose.scale.yml`: cenários de execução.
- `README.Docker.md`, `VALIDACAO_REQUISITOS.md`: documentação complementar.

## Conceitos Básicos de Docker

- **Imagem**: modelo imutável com runtime + sua aplicação.
- **Container**: instância em execução da imagem.
- **Docker Compose**: descreve múltiplos serviços que rodam juntos (API, banco, nginx, etc.).
- **Volumes**: persistem dados (ex.: banco) entre recreações de containers.
- **Rede interna**: serviços se comunicam pelos nomes definidos no `docker-compose.yml` (ex.: `api`, `sqlserver`).

## Pré-requisitos

1. Docker Desktop (Windows/macOS) ou Docker Engine + Compose v2 (Linux).
2. 4 GB de RAM livre.
3. Opcional: .NET 9 SDK e Node.js para execução fora de Docker.
4. Copiar `.env.example` para `.env` na raiz e preencher:
   ```env
   DB_PASSWORD=UmaSenhaForteAqui
   JWT_SECRET_KEY=UmaChaveCom32CaracteresNoMinimo
   JWT_ISSUER=PagueVeloz
   JWT_AUDIENCE=PagueVelozUsers
   NGINX_HTTP_PORT=80
   NGINX_HTTPS_PORT=443
   ```

## Migrações do Banco (Entity Framework)

Executar uma vez para criar/aplicar migrations:

```powershell
cd PagueVeloz
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project PagueVeloz.Infrastructure --startup-project PagueVeloz
dotnet ef database update --project PagueVeloz.Infrastructure --startup-project PagueVeloz
cd ..
```

Se as migrations já existem, basta rodar `dotnet ef database update`. Também é possível executar dentro do container (ver seção “Comandos Úteis”).

## Subindo API + Banco + Nginx (Docker)

Na raiz do repositório:

```powershell
docker compose up -d --build
docker compose ps
docker compose logs -f api
```

Serviços criados:

- `pagueveloz-db`: SQL Server (porta externa definida em `.env`, padrão 1433).
- `pagueveloz-api`: API ASP.NET Core (porta interna 8080).
- `pagueveloz-nginx`: proxy reverso (expondo portas 80/443 para o host).

Endpoints via Nginx:

- API: `http://localhost/api`
- Health check: `http://localhost/health`
- Swagger (quando habilitado): `http://localhost/swagger`

## Dockerizando o Frontend

1. Criar `PagueVeloz.Frontend/Dockerfile` (exemplo):

   ```Dockerfile
   FROM node:18-alpine AS build
   WORKDIR /app
   COPY package*.json ./
   RUN npm install
   COPY . .
   RUN npm run build

   FROM nginx:alpine
   COPY --from=build /app/dist /usr/share/nginx/html
   ```

2. Adicionar serviço ao `docker-compose.yml`:

   ```yaml
   frontend:
     build:
       context: ./PagueVeloz.Frontend
       dockerfile: Dockerfile
     environment:
       - VITE_API_BASE_URL=http://nginx/api
     depends_on:
       - api
     networks:
       - pagueveloz-network
   ```

3. Atualizar `nginx/nginx.conf` e `conf.d/pagueveloz.conf` para rotear `/` para o upstream `frontend` e `/api` para `api_backend`.

4. Recriar containers:

   ```powershell
   docker compose down
   docker compose up -d --build
   ```

Após os ajustes, o frontend estará disponível em `http://localhost` e continuará consumindo a API via Nginx.

## Comandos Úteis

```powershell
# Ver logs
docker compose logs -f
docker compose logs -f api

# Health check
curl http://localhost/health

# Rodar migrations dentro do container
docker compose exec api dotnet ef database update --project PagueVeloz.Infrastructure --startup-project PagueVeloz

# Escalar API em produção (exemplo com override)
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=3

# Parar containers
docker compose down
docker compose down -v  # remove volumes (apaga dados!)
```

## Diagnóstico Rápido

- **API não inicia**: `docker compose logs api`, confirme se o banco ficou saudável.
- **Erro de banco**: `docker compose logs sqlserver`, testar conexão `docker compose exec api ping sqlserver`.
- **Porta ocupada**: verificar com `netstat`/`lsof` (porta 80) e ajustar variáveis `NGINX_HTTP_PORT`/`NGINX_HTTPS_PORT`.

## Resumo do Fluxo de Deploy

1. Configurar `.env` com senhas e chaves.
2. Garantir migrations aplicadas.
3. Executar `docker compose up -d --build`.
4. (Opcional) Dockerizar frontend e ajustar Nginx para servir a SPA.
5. Validar `http://localhost` (frontend) e `http://localhost/api`.

## Recursos Adicionais

- Documentação Docker: <https://docs.docker.com/>
- Documentação ASP.NET + Docker: <https://learn.microsoft.com/aspnet/core/host-and-deploy/docker/>
- Documentação Nginx: <https://nginx.org/en/docs/>


