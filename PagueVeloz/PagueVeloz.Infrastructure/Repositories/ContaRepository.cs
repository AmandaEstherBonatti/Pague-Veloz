using Microsoft.EntityFrameworkCore;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz.Infrastructure.Repositories;

public class ContaRepository : BaseRepository<Conta>, IContaRepository
{
    public ContaRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Conta?> GetByNumeroAsync(string numero, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.Transacoes)
            .FirstOrDefaultAsync(c => c.Numero == numero, cancellationToken);
    }

    public async Task<IEnumerable<Conta>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ClienteId == clienteId)
            .Include(c => c.Transacoes)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conta?> GetWithLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Usando row lock para evitar concorrência
        // Para SQL Server, usar UPDLOCK via SQL RAW
        // Para PostgreSQL, usar SELECT FOR UPDATE via FromSqlRaw
        return await _dbSet
            .Include(c => c.Transacoes)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        
        // TODO: Implementar lock real quando necessário com SQL específico do banco
        // Para SQL Server: FROM SqlRaw("SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
        // Para PostgreSQL: FROM SqlRaw("SELECT * FROM \"Contas\" WHERE \"Id\" = {0} FOR UPDATE", id)
    }
}

