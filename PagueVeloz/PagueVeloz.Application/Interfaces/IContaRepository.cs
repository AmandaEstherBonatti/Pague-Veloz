using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Application.Interfaces;

public interface IContaRepository : IRepository<Conta>
{
    Task<Conta?> GetByNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<Conta>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<Conta?> GetWithLockAsync(Guid id, CancellationToken cancellationToken = default);
}

