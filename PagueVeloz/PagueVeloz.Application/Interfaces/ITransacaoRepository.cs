using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.ValueObjects;

namespace PagueVeloz.Application.Interfaces;

public interface ITransacaoRepository : IRepository<Transacao>
{
    Task<Transacao?> GetByReferenceIdAsync(ReferenceId referenceId, CancellationToken cancellationToken = default);
    Task<bool> ExisteReferenceIdAsync(ReferenceId referenceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transacao>> GetByContaIdAsync(Guid contaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transacao>> GetByContaIdAndPeriodoAsync(
        Guid contaId,
        DateTime dataInicio,
        DateTime dataFim,
        CancellationToken cancellationToken = default);
}

