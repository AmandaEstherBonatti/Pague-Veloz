using Microsoft.EntityFrameworkCore;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz.Infrastructure.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Cliente?> GetByUsuarioAsync(string usuario, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contas)
            .FirstOrDefaultAsync(c => c.Usuario == usuario, cancellationToken);
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExisteUsuarioAsync(string usuario, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.Usuario == usuario, cancellationToken);
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.Email == email, cancellationToken);
    }
}

