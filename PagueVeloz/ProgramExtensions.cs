using Microsoft.EntityFrameworkCore;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz;

public static class ProgramExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Verifica se há migrations pendentes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                // Se não houver migrations configuradas, garante a criação do banco
                await context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error applying database migrations");
            // Não falha a aplicação se migrations falharem
            // Em produção, considere usar um job separado para migrations
        }
    }
}

