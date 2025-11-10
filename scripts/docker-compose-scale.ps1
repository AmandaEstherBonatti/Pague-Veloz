# Script PowerShell para escalar a aplicação PagueVeloz

param(
    [int]$Replicas = 3
)

Write-Host "🚀 Escalando PagueVeloz para $Replicas instâncias..." -ForegroundColor Cyan

# Verificar se os serviços estão rodando
$services = docker-compose ps
if ($services -notmatch "Up") {
    Write-Host "❌ Serviços não estão rodando. Iniciando..." -ForegroundColor Yellow
    docker-compose up -d
}

# Escalar API
Write-Host "📈 Escalando API para $Replicas instâncias..." -ForegroundColor Green
docker-compose up -d --scale api=$Replicas --no-recreate

# Aguardar health checks
Write-Host "⏳ Aguardando health checks..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar status
Write-Host "📊 Status dos serviços:" -ForegroundColor Cyan
docker-compose ps

Write-Host "✅ Escalagem concluída!" -ForegroundColor Green
Write-Host "🌐 Acesse: http://localhost/api/health" -ForegroundColor Cyan

