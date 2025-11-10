# PowerShell script para build e deploy da aplicação Docker

Write-Host "🐳 Building PagueVeloz Docker images..." -ForegroundColor Cyan

# Build da API
Write-Host "📦 Building API image..." -ForegroundColor Yellow
docker build -t pagueveloz-api:latest ./PagueVeloz

Write-Host "✅ Build completed successfully!" -ForegroundColor Green

# Para executar:
# docker-compose up -d

# Para escalar:
# docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale api=3

