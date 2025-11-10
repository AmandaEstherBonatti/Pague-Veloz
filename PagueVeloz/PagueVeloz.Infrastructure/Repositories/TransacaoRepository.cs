using Microsoft.EntityFrameworkCore;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.ValueObjects;
using PagueVeloz.Infrastructure.Persistence;

namespace PagueVeloz.Infrastructure.Repositories;

public class TransacaoRepository : BaseRepository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Transacao?> GetByReferenceIdAsync(ReferenceId referenceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Conta)
            .Include(t => t.ContaDestino)
            .FirstOrDefaultAsync(t => t.ReferenceId.Valor == referenceId.Valor, cancellationToken);
    }

    public async Task<bool> ExisteReferenceIdAsync(ReferenceId referenceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t => t.ReferenceId.Valor == referenceId.Valor, cancellationToken);
    }

    public async Task<IEnumerable<Transacao>> GetByContaIdAsync(Guid contaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ContaId == contaId)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transacao>> GetByContaIdAndPeriodoAsync(
        Guid contaId,
        DateTime dataInicio,
        DateTime dataFim,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ContaId == contaId &&
                       t.DataCriacao >= dataInicio &&
                       t.DataCriacao <= dataFim)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync(cancellationToken);
    }
}

